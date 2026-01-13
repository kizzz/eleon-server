using EleonsoftModuleCollector.Commons.Module.Messages.Identity;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.SettingManagement;
using VPortal.Identity.Module.Permissions;
using VPortal.TenantManagement.Module.Permissions;

namespace VPortal.TenantManagement.Module.Settings
{
  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public class IdentitySettingAppService : TenantManagementAppService, IIdentitySettingAppService
  {
    private readonly IVportalLogger<IdentitySettingAppService> logger;
    private readonly VportalPermissionHelper permissionHelper;
    private readonly IdentitySettingsManager identitySettingsManager;

    public IdentitySettingAppService(
        IVportalLogger<IdentitySettingAppService> logger,
        VportalPermissionHelper permissionHelper,
        IdentitySettingsManager identitySettingsManager)
    {
      this.logger = logger;
      this.permissionHelper = permissionHelper;
      this.identitySettingsManager = identitySettingsManager;
    }

    public async Task<List<IdentitySettingDto>> GetIdentitySettings()
    {
      List<IdentitySettingDto> result = null;
      try
      {
        //await permissionHelper.EnsureAdmin();
        var settings = await identitySettingsManager.GetIdentitySettings();
        result = ObjectMapper.Map<List<IdentitySetting>, List<IdentitySettingDto>>(settings);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> SetIdentitySettings(SetIdentitySettingsRequest request)
    {
      bool result = false;
      try
      {
        await permissionHelper.EnsureAdmin();
        var settings = ObjectMapper.Map<List<IdentitySettingDto>, List<IdentitySetting>>(request.Settings);
        await identitySettingsManager.SetIdentitySettings(settings);
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
