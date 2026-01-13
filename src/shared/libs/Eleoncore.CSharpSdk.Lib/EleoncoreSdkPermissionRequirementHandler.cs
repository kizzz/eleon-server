using EleonsoftProxy.Api;
using Logging.Module;
using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Security.Claims;

namespace Volo.Abp.Authorization;

public class EleoncoreSdkPermissionRequirementHandler : AuthorizationHandler<EleoncoreSdkPermissionRequirement>, IAuthorizationHandler
{
  private readonly IDistributedEventBus _eventBus;
  private readonly ILogger<EleoncoreSdkPermissionRequirementHandler> _logger;
  private readonly IMemoryCache _memoryCache;
  private readonly IFeaturePermissionListApi _featurePermissionListApi;

  public EleoncoreSdkPermissionRequirementHandler(IServiceProvider serviceProvider)
  {
    _eventBus = serviceProvider.GetRequiredService<IDistributedEventBus>();
    _logger = serviceProvider.GetRequiredService<ILogger<EleoncoreSdkPermissionRequirementHandler>>();
    _memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
    _featurePermissionListApi = serviceProvider.GetRequiredService<IFeaturePermissionListApi>();
  }

  protected override async Task HandleRequirementAsync(
      AuthorizationHandlerContext context,
      EleoncoreSdkPermissionRequirement requirement)
  {
    bool isApiKey = false;

    var apiKeyId = context.User.FindFirstValue("client_key_id"); // todo how to ensure is it really api key
    if (!string.IsNullOrEmpty(apiKeyId))
    {
      isApiKey = true;
    }

    var userIdString = context.User.FindFirst("sub")?.Value;
    userIdString = string.IsNullOrEmpty(userIdString) ? context.User.FindFirst(AbpClaimTypes.UserId)?.Value : userIdString;
    userIdString ??= string.Empty;

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

    // STEP 3: replace permission names with booleans
    var expressionWithBooleans =
        PermissionExpressionHelper.ReplacePermissionsWithValues(requirement.PermissionName, permissionValues);

    // STEP 4: evaluate the final expression
    var isGranted = PermissionExpressionHelper.EvaluateBooleanExpression(expressionWithBooleans);

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
    _featurePermissionListApi.UseApiAuth();
    var response = await _featurePermissionListApi.TenantManagementFeaturePermissionListGetAsync(providerName, providerKey);

    var grants = response.Ok()?.Groups?
        .SelectMany(x => x.Permissions)
        .ToList() ?? [];

    var result = new Dictionary<string, bool>();

    foreach (var permission in permissions)
    {
      result[permission] = grants.FirstOrDefault(x => x.Name == permission)?.IsGranted ?? false;
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
