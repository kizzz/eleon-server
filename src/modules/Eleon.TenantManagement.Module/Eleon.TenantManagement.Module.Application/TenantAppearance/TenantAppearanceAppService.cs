using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Permissions;

namespace VPortal.TenantManagement.Module.TenantAppearance
{
  [Authorize]
  public class TenantAppearanceAppService : TenantManagementAppService, ITenantAppearanceAppService
  {
    private readonly IVportalLogger<TenantAppearanceAppService> logger;
    private readonly VportalPermissionHelper permissionHelper;
    private readonly TenantAppearanceDomainService domainService;

    public TenantAppearanceAppService(
        IVportalLogger<TenantAppearanceAppService> logger,
        VportalPermissionHelper permissionHelper,
        TenantAppearanceDomainService domainService)
    {
      this.logger = logger;
      this.permissionHelper = permissionHelper;
      this.domainService = domainService;
    }

    [AllowAnonymous]
    public async Task<TenantAppearanceSettingDto> GetTenantAppearanceSettings()
    {
      TenantAppearanceSettingDto result = null;
      try
      {
        var setting = await domainService.GetCurrentTenantAppearanceSetting();
        result = new()
        {
          LightLogo = setting?.LightLogo,
          LightIcon = setting?.LightIcon,
          DarkLogo = setting?.DarkLogo,
          DarkIcon = setting?.DarkIcon,
        };
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
    public async Task<bool> UpdateTenantAppearanceSettings(UpdateTenantAppearanceSettingRequest request)
    {
      bool result = false;
      try
      {
        await permissionHelper.EnsureAdmin();
        await domainService.SetTenantAppearanceSettingsWithReplication(
            CurrentTenant.Id,
            request.LightLogo,
            request.DarkLogo,
            request.LightIcon,
            request.DarkIcon);

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
