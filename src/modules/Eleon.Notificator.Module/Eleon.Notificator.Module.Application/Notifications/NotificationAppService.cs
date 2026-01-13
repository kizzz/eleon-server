using Common.Module.Constants;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Microsoft.AspNetCore.Authorization;

namespace VPortal.Notificator.Module.Notifications
{

  [Authorize]
  public class NotificationAppService : NotificatorModuleAppService, INotificationAppService
  {
    private readonly IVportalLogger<NotificationAppService> logger;
    private readonly NotificationMananger notificationDomainService;

    public NotificationAppService(
        IVportalLogger<NotificationAppService> logger,
        NotificationMananger notificationDomainService
    )
    {
      this.logger = logger;
      this.notificationDomainService = notificationDomainService;
    }

    public async Task<bool> SendAsync(NotificationDto input)
    {

      try
      {
        var notification = ConvertFromDto(input);

        await notificationDomainService.SendAsync(notification);

        return true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        return false;
      }
      finally
      {
      }
    }

    public async Task<bool> SendBulkAsync(List<NotificationDto> input)
    {

      try
      {
        var result = true;

        foreach (var item in input)
        {
          try
          {
            var notification = ConvertFromDto(item);
            await notificationDomainService.SendAsync(notification);
          }
          catch (Exception ex)
          {
            logger.CaptureAndSuppress(ex);
            result = false;
          }
        }

        return result;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        return false;
      }
      finally
      {
      }
    }

    private EleonsoftNotification ConvertFromDto(NotificationDto input)
    {
      var notification = new EleonsoftNotification
      {
        Id = input.Id,
        Message = input.Message,
        Priority = input.Priority,
        Recipients = input.Recipients?.Select(r => new RecipientEto
        {
          RefId = r.RefId,
          RecipientAddress = r.RecipientAddress,
          Type = r.Type
        }).ToList() ?? new List<RecipientEto>(),
        RunImmidiate = input.RunImmidiate
      };

      switch (input.Type)
      {
        case NotificationType.Email:
          notification.Type = new EmailNotificationType
          {
            IsHtml = input.IsHtml,
            Attachments = input.Attachments,
            Subject = input.Subject
          };
          break;
        case NotificationType.Message:
          notification.Type = new MessageNotificationType
          {
            ApplicationName = input.ApplicationName,
            IsLocalizedData = input.IsLocalizedData,
            IsRedirectEnabled = input.IsRedirectEnabled,
            TemplateName = input.TemplateName,
            RedirectUrl = input.RedirectUrl,
            DataParams = input.DataParams
          };
          break;
        case NotificationType.System:
          notification.Type = new SystemNotificationType
          {
            LogLevel = input.LogLevel,
            WriteLog = input.WriteLog,
            ExtraProperties = input.ExtraProperties ?? new Dictionary<string, string>()
          };
          break;
        case NotificationType.Push:
          notification.Type = new PushNotificationType
          {
            ApplicationName = input.ApplicationName,
            IsLocalizedData = input.IsLocalizedData,
            DataParams = input.DataParams
          };
          break;
        case NotificationType.TwoFactor:
          notification.Type = new TwoFactorNotificationType
          {
            UserName = input.Recipients?.FirstOrDefault()?.RecipientAddress,
          };
          break;
        case NotificationType.Sms:
          notification.Type = new SmsNotificationType
          {

          };
          break;
        case NotificationType.Social:
          notification.Type = new SocialNotificationType
          {
            Platform = input.Platform,
            ChannelId = input.ChannelId
          };
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      return notification;
    }
  }

}
