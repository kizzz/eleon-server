using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.Notificator.Module.EventServices
{
  public class NotificatorRequestedBulkEventService :
      IDistributedEventHandler<NotificatorRequestedBulkMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<NotificatorRequestedBulkEventService> logger;
    private readonly NotificationMananger notificationDomainService;

    public NotificatorRequestedBulkEventService(
        IVportalLogger<NotificatorRequestedBulkEventService> logger,
        NotificationMananger notificationDomainService)
    {
      this.logger = logger;
      this.notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(NotificatorRequestedBulkMsg eventData)
    {
      try
      {
        foreach (var msg in eventData.Messages)
        {
          try
          {
            var notification = msg.Notification;

            await notificationDomainService.SendAsync(notification);
          }
          catch (Exception ex)
          {
            logger.Log.LogError(ex, "Failed to add notification: {0}", msg?.Notification?.Id);
            logger.CaptureAndSuppress(ex);
          }
        }
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
      }
      finally
      {
      }
    }
  }
}
