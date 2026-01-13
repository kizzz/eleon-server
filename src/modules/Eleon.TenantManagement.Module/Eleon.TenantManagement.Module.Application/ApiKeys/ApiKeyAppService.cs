using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using ModuleCollector.Identity.Module.Identity.Module.Application.Contracts.ApiKeys;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.Identity.Module;
using VPortal.Identity.Module.DomainServices;
using VPortal.Identity.Module.Entities;
using VPortal.Identity.Module.Permissions;
using VPortal.TenantManagement.Module;

namespace ModuleCollector.Identity.Module.Identity.Module.Application.ApiKeys
{
  [Authorize(IdentityPermissions.ApiKey.Default)]
  public class ApiKeyAppService : TenantManagementAppService, IApiKeyAppService
  {
    private readonly IVportalLogger<ApiKeyAppService> logger;
    private readonly ApiKeyDomainService apiKeyDomainService;

    public ApiKeyAppService(IVportalLogger<ApiKeyAppService> logger, ApiKeyDomainService apiKeyDomainService)
    {
      this.logger = logger;
      this.apiKeyDomainService = apiKeyDomainService;
    }

    public async Task<IdentityApiKeyDto> AddSdkKeyAsync(string name)
    {
      IdentityApiKeyDto response = null;
      try
      {
        var key = await apiKeyDomainService.AddApiKeyAsync(name, GuidGenerator.Create().ToString(), ApiKeyType.SDK, true);
        response = ObjectMapper.Map<ApiKeyEntity, IdentityApiKeyDto>(key);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return response;
    }

    public async Task<IdentityApiKeyDto> AddIdentityApiKeyAsync(CreateApiKeyDto request)
    {
      try
      {
        var key = await apiKeyDomainService.AddApiKeyAsync(request.Name, request.RefId, request.Type, request.AllowAuthorize, request.Data, request.ExpiresAt);
        return ObjectMapper.Map<ApiKeyEntity, IdentityApiKeyDto>(key);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }

    public async Task<List<IdentityApiKeyDto>> GetApiKeysAsync(ApiKeyRequestDto request)
    {
      List<IdentityApiKeyDto> response = null;
      try
      {
        var keys = await apiKeyDomainService.GetApiKeys(request.KeyTypes);
        response = ObjectMapper.Map<List<ApiKeyEntity>, List<IdentityApiKeyDto>>(keys);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return response;
    }

    public async Task<IdentityApiKeyDto> UpdateAsync(UpdateApiKeyDto request)
    {
      try
      {
        var key = await apiKeyDomainService.UpdateAsync(request.Id, request.Name, request.RefId, request.AllowAuthorize, request.Data);
        return ObjectMapper.Map<ApiKeyEntity, IdentityApiKeyDto>(key);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }

    public async Task RemoveApiKeyAsync(Guid id)
    {
      try
      {
        await apiKeyDomainService.RemoveApiKeyAsync(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<IdentityApiKeyDto> GetByIdAsync(Guid id)
    {
      try
      {
        var key = await apiKeyDomainService.FindByIdAsync(id);
        return ObjectMapper.Map<ApiKeyEntity, IdentityApiKeyDto>(key);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }
  }
}
