using Common.Module.Constants;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.Notificator.Module.DomainServices;
using VPortal.Notificator.Module.Entities;

namespace VPortal.Notificator.Module.UserNotificationSettings
{
  public class UserNotificationSettingsAppService : NotificatorModuleAppService, IUserNotificationSettingsAppService
  {
    private readonly IVportalLogger<IUserNotificationSettingsAppService> logger;
    private readonly UserNotificationSettingsDomainService domainService;

    public UserNotificationSettingsAppService(
        IVportalLogger<IUserNotificationSettingsAppService> logger,
        UserNotificationSettingsDomainService domainService
    )
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<List<UserNotificationSettingsDto>> GetUserNotificationSettings()
    {
      List<UserNotificationSettingsDto> result = null;
      try
      {
        var entities = await domainService.GetUserNotificationSettings(CurrentUser.Id.Value);
        result = ObjectMapper.Map<List<UserNotificationSettingsEntity>, List<UserNotificationSettingsDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> SetUserNotificationSettings(NotificationSourceType sourceType, bool sendNative, bool sendEmail)
    {
      bool result = false;
      try
      {
        await domainService.SetUserNotificationSettings(CurrentUser.Id.Value, sourceType, sendNative, sendEmail);
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
