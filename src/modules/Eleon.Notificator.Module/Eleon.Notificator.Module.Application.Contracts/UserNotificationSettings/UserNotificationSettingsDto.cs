using Common.Module.Constants;

namespace VPortal.Notificator.Module.UserNotificationSettings
{
  public class UserNotificationSettingsDto
  {
    public NotificationSourceType SourceType { get; set; }
    public bool SendNative { get; set; }
    public bool SendEmail { get; set; }
  }
}
