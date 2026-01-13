using Common.EventBus.Module;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Managers;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.Notificator.Module.EventServices
{

  public class AddNotificationEventService :
      IDistributedEventHandler<AddNotificationMsg>,
      IDistributedEventHandler<AddNotificationsMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<AddNotificationEventService> logger;
    private readonly NotificationMananger notificationDomainService;
    private readonly IResponseContext _responseContext;

    public AddNotificationEventService(
        IVportalLogger<AddNotificationEventService> logger,
        NotificationMananger notificationDomainService,
        IResponseContext responseContext
    )
    {
      this.logger = logger;
      this.notificationDomainService = notificationDomainService;
      _responseContext = responseContext;
    }

    public async Task HandleEventAsync(AddNotificationMsg eventData)
    {
      var response = new AddNotificationsResponseMsg
      {
        Success = true
      };
      try
      {
        var notification = eventData.Notification;

        await notificationDomainService.SendAsync(notification);
      }
      catch (Exception e)
      {
        response.Success = false;
        logger.CaptureAndSuppress(e);
      }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }

    public async Task HandleEventAsync(AddNotificationsMsg eventData)
    {
      var response = new AddNotificationsResponseMsg
      {
        Success = true
      };
      try
      {
        foreach (var notification in eventData.Notifications)
        {
          try
          {
            await notificationDomainService.SendAsync(notification);
          }
          catch (Exception ex)
          {
            logger.CaptureAndSuppress(ex);
          }
        }
      }
      catch (Exception e)
      {
        response.Success = false;
        logger.CaptureAndSuppress(e);
      }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }
  }
}
