using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Infrastructure.Module.Result;
using VPortal.TenantManagement.Module;
using VPortal.TenantManagement.Module.UserSettings;

namespace Core.Infrastructure.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/CoreInfrastructure/UserSettings")]
  public class UserSettingsController : TenantManagementController, IUserSettingsAppService
  {
    private readonly IUserSettingsAppService appService;
    private readonly IVportalLogger<UserSettingsController> logger;

    public UserSettingsController(
        IUserSettingsAppService appService,
        IVportalLogger<UserSettingsController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpGet("GetAppearanceSetting")]
    public async Task<ResultDto<string>> GetAppearanceSetting(string appId)
    {

      var response = await appService.GetAppearanceSetting(appId);


      return response;
    }

    [HttpPost("SetAppearanceSetting")]

    public async Task<ResultDto<string>> SetAppearanceSetting(string appearanceSettingsDto, string appId)
    {

      var response = await appService.SetAppearanceSetting(appearanceSettingsDto, appId);


      return response;
    }

    [HttpPost("SetUserSettings")]
    public async Task<UserSettingDto> SetUserSettings(UserSettingDto userSettingDto)
    {

      var response = await appService.SetUserSettings(userSettingDto);


      return response;
    }

    [HttpGet("GetUserSettingsByUserId")]
    public async Task<UserSettingDto> GetUserSettingByUserId(Guid userId)
    {

      var response = await appService.GetUserSettingByUserId(userId);


      return response;
    }

    [HttpGet("GetCurrentUserSetting")]
    public async Task<string> GetCurrentUserSettingAsync(string name)
    {

      var response = await appService.GetCurrentUserSettingAsync(name);


      return response;
    }

    [HttpPost("SetCurrentUserSetting")]
    public async Task SetCurrentUserSettingAsync(string name, string value)
    {

      await appService.SetCurrentUserSettingAsync(name, value);

    }

    [HttpGet("GetUserSetting")]
    public async Task<string> GetUserSettingAsync(Guid userId, string name)
    {

      var response = await appService.GetUserSettingAsync(userId, name);


      return response;
    }

    [HttpPost("SetUserSetting")]
    public async Task SetUserSettingAsync(Guid userId, string name, string value)
    {

      await appService.SetUserSettingAsync(userId, name, value);

    }
  }
}
