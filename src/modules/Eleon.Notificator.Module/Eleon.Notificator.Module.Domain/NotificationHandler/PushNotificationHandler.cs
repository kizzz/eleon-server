using Common.Module.Constants;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.Notificators.Implementations;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.NotificationHandler;
public class PushNotificationHandler : INotificationHandler<PushNotificationType>, ITransientDependency
{
  private readonly PushNotificator _pushNotificator;
  private readonly NotificatorHelperService _notificatorHelperService;

  public PushNotificationHandler(PushNotificator pushNotificator, NotificatorHelperService notificatorHelperService)
  {
    _pushNotificator = pushNotificator;
    _notificatorHelperService = notificatorHelperService;
  }

  public async Task HandleAsync(EleonsoftNotification notification, PushNotificationType type)
  {
    var userIds = new List<Guid>();

    foreach (var recipient in notification.Recipients)
    {
      var resolvedIds = await _notificatorHelperService.ResolveRecepientIdsAsync(recipient.Type, recipient.RefId);
      userIds.AddRange(resolvedIds);
    }

    await _pushNotificator.SendPushAsync(userIds, notification.Message, type.DataParams, type.IsLocalizedData, null, type.ApplicationName, notification.Priority);
  }
}
