using EleonsoftProxy.Model;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftApiSdk.Helpers;
public static class NotificationHelper
{
  public static NotificatorNotificationDto ToDto(this EleonsoftNotification notification)
  {
    var dto = new NotificatorNotificationDto
    {
      Id = notification.Id,
      Message = notification.Message,
      Priority = (EleonsoftSdkNotificationPriority)notification.Priority,
      Recipients = notification.Recipients?.Select(r => new NotificatorNotificatorRecepientDto { RecipientAddress = r.RecipientAddress, RefId = r.RefId, Type = (EleoncoreNotificatorRecepientType)r.Type }).ToList(),
      RunImmidiate = notification.RunImmidiate,
    };

    dto.ExtraProperties = notification.ExtraProperties;
    if (notification.Type is SystemNotificationType system)
    {
      dto.Type = EleoncoreNotificationType.System;
      dto.LogLevel = (EleonSystemLogLevel)system.LogLevel;
      dto.ExtraProperties = system.ExtraProperties;
      dto.WriteLog = system.WriteLog;
    }
    else if (notification.Type is EmailNotificationType email)
    {
      dto.Type = EleoncoreNotificationType.Email;
      dto.IsHtml = email.IsHtml;
      dto.Subject = email.Subject;
      dto.Attachments = email.Attachments;
    }
    else if (notification.Type is MessageNotificationType message)
    {
      dto.Type = EleoncoreNotificationType.Message;
      dto.ApplicationName = message.ApplicationName;
      dto.IsLocalizedData = message.IsLocalizedData;
      dto.IsRedirectEnabled = message.IsRedirectEnabled;
      dto.TemplateName = message.TemplateName;
      dto.RedirectUrl = message.RedirectUrl;
      dto.LanguageKeyParams = message.LanguageKeyParams;
    }
    else if (notification.Type is PushNotificationType push)
    {
      dto.Type = EleoncoreNotificationType.Push;
      dto.ApplicationName = push.ApplicationName;
      dto.IsLocalizedData = push.IsLocalizedData;
      dto.LanguageKeyParams = push.LanguageKeyParams;
    }
    else if (notification.Type is SmsNotificationType sms)
    {
      dto.Type = EleoncoreNotificationType.Sms;
    }
    else if (notification.Type is TwoFactorNotificationType twoFactor)
    {
      dto.Type = EleoncoreNotificationType.TwoFactor;
    }
    else if (notification.Type is SocialNotificationType social)
    {
      dto.Type = EleoncoreNotificationType.Social;
      dto.Platform = social.Platform;
      dto.ChannelId = social.ChannelId;
    }

    return dto;
  }
}
