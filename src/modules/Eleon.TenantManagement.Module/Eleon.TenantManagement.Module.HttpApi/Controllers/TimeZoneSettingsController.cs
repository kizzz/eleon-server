using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.SettingManagement;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/TimeZoneSettings")]
  public class TimeZoneSettingsController : TenantManagementController, ITimeZoneSettingsAppService
  {
    private readonly IVportalLogger<TimeZoneSettingsController> logger;
    private readonly ITimeZoneSettingsAppService timeZoneSettingsAppService;

    public TimeZoneSettingsController(
        IVportalLogger<TimeZoneSettingsController> logger,
        ITimeZoneSettingsAppService timeZoneSettingsAppService)
    {
      this.logger = logger;
      this.timeZoneSettingsAppService = timeZoneSettingsAppService;
    }

    [HttpGet("GetTimezonesAsync")]
    public async Task<List<NameValue>> GetTimezonesAsync()
    {

      var response = await timeZoneSettingsAppService.GetTimezonesAsync();


      return response;
    }

    [HttpPost("GetTimezonesAsync")]
    public async Task UpdateAsync(string timezone)
    {

      await timeZoneSettingsAppService.UpdateAsync(timezone);

    }

    [HttpGet("GetAsync")]
    public async Task<string> GetAsync()
    {

      var response = await timeZoneSettingsAppService.GetAsync();

      return response;
    }
  }
}
