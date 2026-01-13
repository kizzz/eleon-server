using EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.TenantSettings;
using EleonsoftModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.ValueObjects;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Permissions;

namespace Core.Infrastructure.Module.TenantSettings
{
  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public class TenantSettingsAppService : TenantManagementAppService, ITenantSettingsAppService
  {
    private readonly IVportalLogger<TenantSettingsAppService> logger;
    private readonly VportalPermissionHelper permissionHelper;
    private readonly TenantSettingsCacheService tenantSettingsCache;
    private readonly TenantSettingDomainService domainService;
    private readonly IDistributedEventBus eventBus;

    public TenantSettingsAppService(
        IVportalLogger<TenantSettingsAppService> logger,
        VportalPermissionHelper permissionHelper,
        TenantSettingsCacheService tenantSettingsCache,
        TenantSettingDomainService domainService,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.permissionHelper = permissionHelper;
      this.tenantSettingsCache = tenantSettingsCache;
      this.domainService = domainService;
      this.eventBus = eventBus;

    }

    public async Task<TenantSystemHealthSettingsDto> GetTenantSystemHealthSettingsAsync()
    {
      try
      {
        var settings = await domainService.GetTenantSystemHealthSettings();
        return ObjectMapper.Map<TenantSystemHealthSettings, TenantSystemHealthSettingsDto>(settings);
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

    public async Task<bool> UpdateTenantSystemHealthSettingsAsync(TenantSystemHealthSettingsDto request)
    {
      try
      {
        var settings = ObjectMapper.Map<TenantSystemHealthSettingsDto, TenantSystemHealthSettings>(request);


        var result = await domainService.UpdateTenantSystemHealthSettingsAsync(settings);

        await eventBus.PublishAsync(new TelemetryStorageProviderChangedMsg
        {
          StorageProviderId = settings.Telemetry.StorageProviderId.ToString()
        });

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

    public async Task<TenantSettingDto> GetTenantSettings(Guid? tenantId)
    {
      TenantSettingDto result = null;
      try
      {
        var entity = await domainService.GetOrCreateTenantSettings(tenantId);
        result = ObjectMapper.Map<TenantSettingEntity, TenantSettingDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }


    public async Task<bool> SetExternalProviderSettings(SetTenantProviderSettingsRequestDto request)
    {
      bool result = false;
      try
      {
        await permissionHelper.EnsureHostAdmin();
        var settingsEntities = ObjectMapper.Map<List<TenantExternalLoginProviderDto>, List<TenantExternalLoginProviderEntity>>(request.Providers);
        await domainService.SetExternalProviderSettings(request.TenantId, settingsEntities);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
