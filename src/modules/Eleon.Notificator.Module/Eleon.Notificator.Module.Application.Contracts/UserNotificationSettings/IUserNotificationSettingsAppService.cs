using Common.Module.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Notificator.Module.UserNotificationSettings
{
  public interface IUserNotificationSettingsAppService : IApplicationService
  {
    Task<bool> SetUserNotificationSettings(NotificationSourceType sourceType, bool sendNative, bool sendEmail);
    Task<List<UserNotificationSettingsDto>> GetUserNotificationSettings();
  }
}
