using Core.Infrastructure.Module.DashboardSettings;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Core.Infrastructure.Module;
using VPortal.Infrastructure.Module;

namespace Core.Infrastructure.Module.Controllers
{
  [Area(InfrastructureRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = InfrastructureRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/DashboardSettings/")]
  public class DashboardSettingController : InfrastructureController, IDashboardSettingAppService
  {
    private readonly IDashboardSettingAppService appService;
    private readonly IVportalLogger<DashboardSettingController> logger;

    public DashboardSettingController(
        IDashboardSettingAppService appService,
        IVportalLogger<DashboardSettingController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpPost("CreateOrUpdateSettings")]
    public async Task<string> UpdateSettings(List<DashboardSettingDto> dashboardSettingDtos, bool setAsDefault)
    {

      var response = await appService.UpdateSettings(dashboardSettingDtos, setAsDefault);


      return response;
    }

    [HttpPost("DeleteDashboardSettings")]
    public async Task<string> DeleteDashboardSettings(Guid dashboardSettingEntityId)
    {

      var response = await appService.DeleteDashboardSettings(dashboardSettingEntityId);


      return response;
    }

    [HttpGet("GetDashboardSettings")]
    public async Task<List<DashboardSettingDto>> GetDashboardSettings()
    {

      var response = await appService.GetDashboardSettings();


      return response;
    }
  }
}
