using Logging.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Shared.Extensions;
using System;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using TenantSettings.Module.Models;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.TenantManagement.Module.Entities;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public class TenantAppearanceDomainService : DomainService
  {
    private readonly IVportalLogger<TenantAppearanceDomainService> logger;
    private readonly IObjectMapper objectMapper;
    private readonly TenantSettingDomainService tenantSettingDomainService;
    private readonly TenantSettingsCacheService tenantSettingsCache;

    public TenantAppearanceDomainService(
        IVportalLogger<TenantAppearanceDomainService> logger,
        IObjectMapper objectMapper,
        TenantSettingDomainService tenantSettingDomainService,
        TenantSettingsCacheService tenantSettingsCache)
    {
      this.logger = logger;
      this.objectMapper = objectMapper;
      this.tenantSettingDomainService = tenantSettingDomainService;
      this.tenantSettingsCache = tenantSettingsCache;
    }

    public async Task<TenantAppearanceSetting> GetCurrentTenantAppearanceSetting()
    {
      TenantAppearanceSetting result = null;
      try
      {
        await tenantSettingsCache.UpdateCache();
        var tenantSetting = await tenantSettingsCache.GetTenantSettings(CurrentTenant.Id);
        result = tenantSetting?.TenantAppearanceSetting;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SetTenantAppearanceSettingsWithReplication(Guid? tenantId, string lightLogo, string darkLogo, string lightIcon, string darkIcon)
    {
      try
      {
        await SetTenantAppearanceSetting(tenantId, lightLogo, darkLogo, lightIcon, darkIcon);

        //if (CurrentTenant.Id != null)
        //{
        //    using (CurrentTenant.ChangeDefault())
        //    {
        //        await SetTenantAppearanceSetting(tenantId, lightLogo, darkLogo, lightIcon, darkIcon);
        //    }
        //}
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task SetTenantAppearanceSetting(Guid? tenantId, string lightLogo, string darkLogo, string lightIcon, string darkIcon)
    {
      var tenantSetting = await tenantSettingDomainService.GetOrCreateTenantSettings(tenantId);
      if (tenantSetting.AppearanceSettings == null)
      {
        tenantSetting.AppearanceSettings = new TenantAppearanceSettingEntity(GuidGenerator.Create())
        {
          DarkLogo = darkLogo,
          DarkIcon = darkIcon,
          LightLogo = lightLogo,
          LightIcon = lightIcon,
        };
      }
      else
      {
        tenantSetting.AppearanceSettings.DarkLogo = darkLogo;
        tenantSetting.AppearanceSettings.DarkIcon = darkIcon;
        tenantSetting.AppearanceSettings.LightLogo = lightLogo;
        tenantSetting.AppearanceSettings.LightIcon = lightIcon;
      }

      await tenantSettingDomainService.UpdateSettings(tenantSetting);
    }
  }
}
