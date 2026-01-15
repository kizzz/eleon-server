using Common.Module.Constants;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.DomainServices;
using VPortal.Notificator.Module.Notificators.Implementations;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.NotificationHandler;
public class MessageNotificationHandler : INotificationHandler<MessageNotificationType>, ITransientDependency
{
  private readonly IVportalLogger<MessageNotificationHandler> _logger;
  private readonly NotificatorHelperService _notificatorHelperService;
  private readonly EmailNotificator _emailNotificator;
  private readonly NotificationLogDomainService _notificationLogDomainService;
  private readonly PushNotificator _pushNotificator;

  public MessageNotificationHandler(
      IVportalLogger<MessageNotificationHandler> logger,
      NotificatorHelperService notificatorHelperService,
      EmailNotificator emailNotificator,
      NotificationLogDomainService notificationLogDomainService,
      PushNotificator pushNotificator
      )
  {
    _logger = logger;
    _notificatorHelperService = notificatorHelperService;
    _emailNotificator = emailNotificator;
    _notificationLogDomainService = notificationLogDomainService;
    _pushNotificator = pushNotificator;
  }

  public async Task HandleAsync(EleonsoftNotification notification, MessageNotificationType type)
  {
    try
    {
      foreach (var recipient in notification.Recipients)
      {
        if (recipient.Type == Common.Module.Constants.NotificatorRecepientType.Direct)
        {
          if (_notificatorHelperService.IsEmail(recipient.RecipientAddress))
          {
            await _emailNotificator.SendEmailAsync(_notificatorHelperService.GetValidatedSubjectOrDefault(string.Empty), notification.Message, [recipient.RecipientAddress], true);
          }
        }
        else
        {
          var userIds = await _notificatorHelperService.ResolveRecepientIdsAsync(recipient.Type, recipient.RefId);

          foreach (var userId in userIds)
          {
            var log = await _notificationLogDomainService.AddNotificationLogAsync(
                userId,
                notification.Message,
                type.IsLocalizedData,
                type.LanguageKeyParams,
                type.ApplicationName,
                type.IsRedirectEnabled,
                type.RedirectUrl,
                notification.Priority);
          }

          await _pushNotificator.SendPushAsync(userIds.Distinct().ToList(), notification.Message, type.LanguageKeyParams, type.IsLocalizedData, type.IsRedirectEnabled ? type.RedirectUrl : null, type.ApplicationName, notification.Priority);
        }
      }
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
    }
  }
}
