using Common.Module.Constants;
using System.Collections.Generic;

namespace VPortal.Notificator.Module.UserNotificationSettings
{
  public record NotificationSourceTypeDefaults(bool SendNative = false, bool SendEmail = false);

  public class UserNotificationSettingsConsts
  {
    public static readonly IReadOnlyDictionary<NotificationSourceType, NotificationSourceTypeDefaults> Defaults = new Dictionary<NotificationSourceType, NotificationSourceTypeDefaults>()
        {
            { NotificationSourceType.Chat, new NotificationSourceTypeDefaults(true, false) },
            { NotificationSourceType.Notification, new NotificationSourceTypeDefaults(true, true) }
        };
  }
}
