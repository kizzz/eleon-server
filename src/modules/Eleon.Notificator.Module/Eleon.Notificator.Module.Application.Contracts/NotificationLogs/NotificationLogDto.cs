using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using System;

namespace VPortal.Notificator.Module.NotificationLogs
{
  public class NotificationLogDto
  {
    public DateTime CreationTime { get; set; }

    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string Content { get; set; }

    public bool IsLocalizedData { get; set; }

    public string LanguageKeyParams { get; set; }

    public string ApplicationName { get; set; }

    public bool IsRedirectEnabled { get; set; }
    public bool IsRead { get; set; }
    public string RedirectUrl { get; set; }
    public NotificationPriority Priority { get; set; }
  }
}
