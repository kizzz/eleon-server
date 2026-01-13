using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.TenantManagement.Module.TenantAppearance;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/TenantAppearance/")]
  public class TenantAppearanceController : TenantManagementController, ITenantAppearanceAppService
  {
    private readonly IVportalLogger<TenantAppearanceController> logger;
    private readonly ITenantAppearanceAppService appService;

    public TenantAppearanceController(
        IVportalLogger<TenantAppearanceController> logger,
        ITenantAppearanceAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetTenantAppearanceSettings")]
    public async Task<TenantAppearanceSettingDto> GetTenantAppearanceSettings()
    {
      var result = await appService.GetTenantAppearanceSettings();
      return result;
    }

    [HttpPost("UpdateTenantAppearanceSettings")]
    public async Task<bool> UpdateTenantAppearanceSettings(UpdateTenantAppearanceSettingRequest request)
    {
      var result = await appService.UpdateTenantAppearanceSettings(request);
      return result;
    }
  }
}
