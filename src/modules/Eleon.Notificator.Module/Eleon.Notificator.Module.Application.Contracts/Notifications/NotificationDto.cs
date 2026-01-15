using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Contracts;
using EleonsoftModuleCollector.Commons.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using System;

namespace VPortal.Notificator.Module.Notifications
{
  public class NotificatorRecepientDto
  {
    public string RefId { get; set; }
    public string RecipientAddress { get; set; }
    public NotificatorRecepientType Type { get; set; }
  }

  public class NotificationDto
  {
    public Guid? Id { get; set; }
    public List<NotificatorRecepientDto> Recipients { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public string Message { get; set; }

    public NotificationType Type { get; set; }
    public bool? RunImmidiate { get; set; }

    // Push and Message type
    public string? ApplicationName { get; set; }
    public bool IsLocalizedData { get; set; } = false;
    public bool IsRedirectEnabled { get; set; }
    public string TemplateName { get; set; }
    public string RedirectUrl { get; set; }
    public List<string> LanguageKeyParams { get; set; }

    // Social type
    public string Platform { get; set; } // e.g., Telegram, WhatsUp, Facebook, Twitter
    public string ChannelId { get; set; }

    // Email type
    public bool IsHtml { get; set; }
    public string Subject { get; set; }
    // Key: FileName, Value: Base64 string
    public Dictionary<string, string> Attachments { get; set; }

    // System Type
    public bool WriteLog { get; set; } = true;
    public SystemLogLevel LogLevel { get; set; } = SystemLogLevel.Info;
    public Dictionary<string, string> ExtraProperties { get; set; } = new Dictionary<string, string>();
  }
}
