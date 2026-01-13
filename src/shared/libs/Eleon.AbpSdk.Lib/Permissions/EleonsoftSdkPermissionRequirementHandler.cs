using Common.EventBus.Module;
using EleonsoftAbp.Auth;
using EleonsoftSdk.Messages.Permissions;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Volo.Abp.Authorization;

public class EleonsoftSdkPermissionRequirementHandler : AuthorizationHandler<EleonsoftSdkPermissionRequirement>, IAuthorizationHandler
{
  internal const string CacheKey = "EleonsoftSdkPermissionCache";
  private const int CacheExpirationMinutes = 60;

  private readonly IDistributedEventBus _eventBus;
  private readonly ILogger<EleonsoftSdkPermissionRequirementHandler> _logger;
  private readonly IMemoryCache _memoryCache;

  public EleonsoftSdkPermissionRequirementHandler(
      IDistributedEventBus eventBus,
      ILogger<EleonsoftSdkPermissionRequirementHandler> logger,
      IMemoryCache memoryCache)
  {
    _eventBus = eventBus;
    _logger = logger;
    _memoryCache = memoryCache;
  }

  protected override async Task HandleRequirementAsync(
      AuthorizationHandlerContext context,
      EleonsoftSdkPermissionRequirement requirement)
  {
    bool isApiKey = false;

    var apiKeyId = context.User.GetApiKeyId() ?? string.Empty; // todo how to ensure is it really api key
    if (!string.IsNullOrEmpty(apiKeyId))
    {
      isApiKey = true;
    }

    var userIdString = context.User.FindFirst("sub")?.Value ?? string.Empty;


    if (string.IsNullOrEmpty(apiKeyId) && string.IsNullOrEmpty(userIdString))
    {
      context.Fail();
      return;
    }

    // STEP 1: extract permission names
    var permissionNames = PermissionExpressionHelper.ExtractPermissionNames(requirement.PermissionName);

    // STEP 2: check permissions with event bus
    var permissionValues = isApiKey ?
        await CheckPermissionsAsync("A", apiKeyId, permissionNames) :
        await CheckPermissionsAsync("U", userIdString, permissionNames);

    _logger.LogDebug("PermissionsValues: {values}", string.Join(";", permissionValues.Select(kvp => $"{kvp.Key}:{kvp.Value}")));

    // STEP 3: replace permission names with booleans
    var expressionWithBooleans =
        PermissionExpressionHelper.ReplacePermissionsWithValues(requirement.PermissionName, permissionValues);

    // STEP 4: evaluate the final expression
    var isGranted = PermissionExpressionHelper.EvaluateBooleanExpression(expressionWithBooleans);

    _logger.LogDebug("Permission expression: {expression} = {isGranted}", expressionWithBooleans, isGranted);

    if (isGranted)
    {
      context.Succeed(requirement);
    }
    else
    {
      context.Fail(new AuthorizationFailureReason(this, $"Required permissions was not granted. Required: {requirement.PermissionName}; Granted: {expressionWithBooleans}"));
    }
  }

  protected virtual async Task<Dictionary<string, bool>> CheckPermissionsAsync(string providerName, string providerKey, List<string> permissions)
  {
    var result = new Dictionary<string, bool>();

    var cache = _memoryCache.GetOrCreate(CacheKey, entry =>
    {
      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes);
      return new ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>();
    });

    var unknownPermissions = new List<string>();

    if (cache.TryGetValue(providerName + providerKey, out var cachedPermissions))
    {
      foreach (var permission in permissions)
      {
        if (cachedPermissions.TryGetValue(permission, out var value))
        {
          result[permission] = value;
        }
        else
        {
          unknownPermissions.Add(permission);
        }
      }
    }
    else
    {
      unknownPermissions.AddRange(permissions);
    }

    if (unknownPermissions.Count > 0)
    {
      var response = await _eventBus.RequestAsync<CheckPermissionResponseMsg>(
        new CheckPermissionRequestMsg
        {
          ProviderName = providerName,
          ProviderKey = providerKey,
          Permissions = unknownPermissions
        });

      if (response == null || !response.IsSuccessful)
      {
        _logger.LogError("Failed to check permissions via event bus for ProviderName: {providerName}, ProviderKey: {providerKey}. Unknown permissions: {unknownPermissions}. Response error message: {errorMessage}",
              providerName, providerKey, string.Join(", ", unknownPermissions), response?.ErrorMessage);
        _memoryCache.Remove(CacheKey);

        foreach (var permission in unknownPermissions)
        {
          result[permission] = false;
        }
      }
      else
      {
        foreach (var permission in unknownPermissions)
        {
          var permissionGrantValue = response.PermissionGrants?.GetOrDefault(permission) ?? false;
          result[permission] = permissionGrantValue;
          if (cache != null && response.IsSuccessful)
          {
            if (!cache.TryGetValue(providerName.ToString() + providerKey, out var existing))
            {
              cache[providerName.ToString() + providerKey] = new ConcurrentDictionary<string, bool>();
            }
            cache[providerName.ToString() + providerKey][permission] = permissionGrantValue;
          }
        }
      }
    }

    return result;
  }
}


public static class PermissionExpressionHelper
{
  public static List<string> ExtractPermissionNames(string expression)
  {
    var delimiters = new[] { "&&", "||", "(", ")" };

    return expression
        .Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrEmpty(x))
        .Distinct()
        .ToList();
  }

  public static string ReplacePermissionsWithValues(
      string expression,
      Dictionary<string, bool> permissionValues)
  {
    expression = Normalize(expression);

    foreach (var kvp in permissionValues)
    {
      expression = ReplaceWholeWord(expression, kvp.Key, kvp.Value.ToString().ToLower());
    }
    return expression;
  }

  public static bool EvaluateBooleanExpression(string expression)
  {
    expression = expression
        .Replace("&&", "AND")
        .Replace("||", "OR");

    var dt = new System.Data.DataTable();
    var result = dt.Compute(expression, null);
    return Convert.ToBoolean(result);
  }

  public static string ReplaceWholeWord(string input, string word, string replacement)
  {
    return System.Text.RegularExpressions.Regex.Replace(
        input,
        $@"\b{System.Text.RegularExpressions.Regex.Escape(word)}\b",
        replacement);
  }

  private static string Normalize(string input)
  {
    if (string.IsNullOrWhiteSpace(input)) return "false";

    var s = input;

    // Put spaces around parens and operators
    s = Regex.Replace(s, @"([()])", " $1 ");
    s = Regex.Replace(s, @"\|\|", " || ");
    s = Regex.Replace(s, @"&&", " && ");
    s = Regex.Replace(s, @"!", " ! ");

    // Collapse multiple spaces
    s = Regex.Replace(s, @"\s+", " ").Trim();
    return s;
  }
}

public class EleonsoftSdkPermissionCacheCleaner :
    IDistributedEventHandler<EntityCreatedEto<UserEto>>,
    IDistributedEventHandler<EntityUpdatedEto<UserEto>>,
    IDistributedEventHandler<EntityDeletedEto<UserEto>>,
    IDistributedEventHandler<EntityUpdatedEto<IdentityRoleEto>>,
    IDistributedEventHandler<EntityCreatedEto<IdentityRoleEto>>,
    IDistributedEventHandler<EntityDeletedEto<IdentityRoleEto>>,
    IDistributedEventHandler<EntityCreatedEto<OrganizationUnitEto>>,
    IDistributedEventHandler<EntityUpdatedEto<OrganizationUnitEto>>,
    IDistributedEventHandler<EntityDeletedEto<OrganizationUnitEto>>,
    IDistributedEventHandler<EntityCreatedEto<EntityEto>>,
    IDistributedEventHandler<EntityUpdatedEto<EntityEto>>,
    IDistributedEventHandler<EntityDeletedEto<EntityEto>>
{
  private readonly IMemoryCache _cache;

  public EleonsoftSdkPermissionCacheCleaner(IMemoryCache cache)
  {
    _cache = cache;
  }

  public Task HandleEventAsync(EntityCreatedEto<UserEto> eventData)
  {
    return CleanCache();
  }

  public Task HandleEventAsync(EntityDeletedEto<UserEto> eventData)
  {
    return CleanCache();
  }

  public Task HandleEventAsync(EntityUpdatedEto<IdentityRoleEto> eventData)
  {
    return CleanCache();
  }

  public Task HandleEventAsync(EntityCreatedEto<IdentityRoleEto> eventData)
  {
    return CleanCache();
  }

  public Task HandleEventAsync(EntityDeletedEto<IdentityRoleEto> eventData)
  {
    return CleanCache();
  }

  public Task HandleEventAsync(EntityCreatedEto<OrganizationUnitEto> eventData)
  {
    return CleanCache();
  }

  public Task HandleEventAsync(EntityUpdatedEto<OrganizationUnitEto> eventData)
  {
    return CleanCache();
  }

  public Task HandleEventAsync(EntityDeletedEto<OrganizationUnitEto> eventData)
  {
    return CleanCache();
  }

  public Task HandleEventAsync(EntityUpdatedEto<UserEto> eventData)
  {
    return CleanCache();
  }

  public Task HandleEventAsync(EntityCreatedEto<EntityEto> eventData)
  {
    if (eventData.Entity.EntityType == typeof(Volo.Abp.PermissionManagement.PermissionGrant).FullName ||
        eventData.Entity.EntityType == typeof(IdentityUserRole).FullName ||
        eventData.Entity.EntityType == typeof(OrganizationUnitRole).FullName ||
        eventData.Entity.EntityType == typeof(IdentityUserOrganizationUnit).FullName)
    {
      return CleanCache();
    }
    return Task.CompletedTask;
  }

  public Task HandleEventAsync(EntityUpdatedEto<EntityEto> eventData)
  {
    if (eventData.Entity.EntityType == typeof(Volo.Abp.PermissionManagement.PermissionGrant).FullName ||
        eventData.Entity.EntityType == typeof(IdentityUserRole).FullName ||
        eventData.Entity.EntityType == typeof(OrganizationUnitRole).FullName ||
        eventData.Entity.EntityType == typeof(IdentityUserOrganizationUnit).FullName)
    {
      return CleanCache();
    }
    return Task.CompletedTask;
  }

  public Task HandleEventAsync(EntityDeletedEto<EntityEto> eventData)
  {
    if (eventData.Entity.EntityType == typeof(Volo.Abp.PermissionManagement.PermissionGrant).FullName ||
        eventData.Entity.EntityType == typeof(IdentityUserRole).FullName ||
        eventData.Entity.EntityType == typeof(OrganizationUnitRole).FullName ||
        eventData.Entity.EntityType == typeof(IdentityUserOrganizationUnit).FullName)
    {
      return CleanCache();
    }
    return Task.CompletedTask;
  }

  private Task CleanCache()
  {
    _cache.Remove(EleonsoftSdkPermissionRequirementHandler.CacheKey);
    return Task.CompletedTask;
  }
}
