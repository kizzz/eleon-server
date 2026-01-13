using Common.Module.Constants;
using EleonsoftAbp.Messages.ApiKey;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TenantSettings.Module.Helpers;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Validation;
using VPortal.Identity.Module.Entities;
using VPortal.Identity.Module.Repositories;

namespace VPortal.Identity.Module.DomainServices
{

  public class ApiKeyDomainService : DomainService
  {
    private readonly IApiKeyRepository apiKeyRepository;
    private readonly IdentityUserManager userManager;
    private readonly MultiTenancyDomainService multiTenancyDomainService;
    private readonly IVportalLogger<ApiKeyDomainService> logger;
    private readonly IDistributedEventBus _eventBus;

    public ApiKeyDomainService(
        IApiKeyRepository apiKeyRepository,
        IdentityUserManager userManager,
        MultiTenancyDomainService multiTenancyDomainService,
        IVportalLogger<ApiKeyDomainService> logger,
        IDistributedEventBus eventBus)
    {
      this.apiKeyRepository = apiKeyRepository;
      this.userManager = userManager;
      this.multiTenancyDomainService = multiTenancyDomainService;
      this.logger = logger;
      _eventBus = eventBus;
    }

    public async Task<ApiKeyEntity> FindByIdAsync(Guid id)
    {
      try
      {
        var key = await apiKeyRepository.FindAsync(id);
        return key;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    public async Task<ClaimsPrincipal> CreateClaimsPrincipal(ApiKeyEntity apiKey)
    {
      var claims = apiKey.GetClaims("client_");

      var result = new ClaimsPrincipal(new ClaimsIdentity(claims, VPortalExtensionGrantsConsts.Names.ApiKeyGrant));
      return result;
    }

    public async Task<ApiKeyEntity> ValidateApiKey(string apiKey, bool autoDetectTenant = false)
    {
      ApiKeyEntity result = null;
      try
      {
        ApiKeyEntity keyEntity;
        if (autoDetectTenant)
        {
          var matches = await multiTenancyDomainService.ForEachTenant(async () => await apiKeyRepository.GetByKey(apiKey));
          keyEntity = matches.SingleOrDefault(x => x != null);
        }
        else
        {
          keyEntity = await apiKeyRepository.GetByKey(apiKey);
        }

        if (keyEntity != null)
        {
          bool expired = keyEntity.ExpiresAt <= DateTime.UtcNow;
          bool invalidated = keyEntity.Invalidated;
          bool invalid = expired || invalidated;
          result = invalid ? null : keyEntity;
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<ApiKeyEntity> AddApiKeyAsync(string name, string subject, ApiKeyType keyType, bool allowAuthorize, string data = null, DateTime? expireDate = null)
    {
      try
      {
        var id = GuidGenerator.Create();
        var keyEntity = new ApiKeyEntity(id, subject, GenerateApiKey())
        {
          ExpiresAt = expireDate,
          Type = keyType,
          Name = await ValidateName(name, keyType, id),
          AllowAuthorize = allowAuthorize,
          KeySecret = GenerateApiKeySecret(),
          Data = data
        };

        var result = await apiKeyRepository.InsertAsync(keyEntity);
        return result;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    public async Task<List<ApiKeyEntity>> GetApiKeys(List<ApiKeyType> keyTypesFilter)
    {
      List<ApiKeyEntity> result = null;
      try
      {
        var keys = await apiKeyRepository.GetListAsync();
        if (keyTypesFilter.IsNullOrEmpty())
        {
          result = keys;
        }
        else
        {
          result = keys.Where(x => keyTypesFilter.Contains(x.Type)).ToList();
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<ApiKeyEntity> UpdateAsync(Guid keyId, string name, string refId, bool allowAuthorize, string data)
    {
      try
      {
        var key = await apiKeyRepository.GetAsync(keyId);

        if (!string.IsNullOrEmpty(name) && name != key.Name)
        {
          key.Name = await ValidateName(name, key.Type, keyId);
        }
        key.AllowAuthorize = allowAuthorize;
        key.Data = data;
        key.RefId = refId;
        var updatedKey = await apiKeyRepository.UpdateAsync(key);
        return updatedKey;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    public async Task RemoveApiKey(ApiKeyType keyType, string subject)
    {
      try
      {
        var key = await apiKeyRepository.GetBySubject(keyType, subject);
        await RemoveApiKeyAsync(key.Id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

    }

    public async Task RemoveApiKeyAsync(Guid id)
    {
      try
      {
        var key = await apiKeyRepository.GetAsync(id);
        if (key != null)
        {
          await apiKeyRepository.DeleteAsync(key);

          await _eventBus.PublishAsync(new ApiKeyDeletedMsg
          {
            IsSuccessfully = true,
            ApiKeyId = id.ToString(),
          });
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    private string GenerateApiKey() => Convert.ToHexString(SHA512.HashData(GuidGenerator.Create().ToString().GetBytes()));

    private string GenerateApiKeySecret() => Convert.ToHexString(SHA512.HashData(GuidGenerator.Create().ToString().GetBytes()));

    private async Task<string> ValidateName(string name, ApiKeyType type, Guid id)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return type.ToString() + "_" + (await apiKeyRepository.GetCountAsync());
      }

      if (!Regex.IsMatch(name, @"^[A-Za-z0-9_]+$"))
      {
        throw new AbpValidationException($"Invalid api key name identifier {name} key can contains only characters [A-Za-z0-9_]");
      }

      return name;
    }
  }
}
