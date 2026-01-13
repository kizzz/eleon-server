using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Notificator.Module.UserNotificationSettings;

namespace VPortal.Notificator.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Notificator/NotificationSettings")]
  public class UserNotificationSettingsController : NotificatorModuleController, IUserNotificationSettingsAppService
  {
    private readonly IUserNotificationSettingsAppService appService;
    private readonly IVportalLogger<UserNotificationSettingsController> logger;

    public UserNotificationSettingsController(
        IUserNotificationSettingsAppService notificationsAppService,
        IVportalLogger<UserNotificationSettingsController> logger)
    {
      this.appService = notificationsAppService;
      this.logger = logger;
    }

    [HttpGet("GetUserNotificationSettings")]
    public async Task<List<UserNotificationSettingsDto>> GetUserNotificationSettings()
    {

      var response = await appService.GetUserNotificationSettings();

      return response;
    }

    [HttpPost("SetUserNotificationSettings")]
    public async Task<bool> SetUserNotificationSettings(NotificationSourceType sourceType, bool sendNative, bool sendEmail)
    {

      var response = await appService.SetUserNotificationSettings(sourceType, sendNative, sendEmail);

      return response;
    }
  }
}
