using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.TenantManagement.Module.Settings;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/IdentitySettings/")]
  public class IdentitySettingController : TenantManagementController, IIdentitySettingAppService
  {
    private readonly IVportalLogger<IdentitySettingController> logger;
    private readonly IIdentitySettingAppService appService;

    public IdentitySettingController(
        IVportalLogger<IdentitySettingController> logger,
        IIdentitySettingAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetIdentitySettings")]
    public async Task<List<IdentitySettingDto>> GetIdentitySettings()
    {
      var result = await appService.GetIdentitySettings();
      return result;
    }

    [HttpPost("SetIdentitySettings")]
    public async Task<bool> SetIdentitySettings(SetIdentitySettingsRequest request)
    {
      var result = await appService.SetIdentitySettings(request);
      return result;
    }
  }
}
