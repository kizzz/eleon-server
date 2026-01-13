using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.PushNotifications;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Shared.ValueObjects;
using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace VPortal.Notificator.Module.PushNotifications
{
  public class PushNotificationHubContext : IPushNotificationHubContext, ITransientDependency
  {
    private readonly IVportalLogger<PushNotificationHubContext> logger;
    private readonly IObjectMapper objectMapper;
    private readonly IPushNotificationAppHubContext notificationHub;

    public PushNotificationHubContext(
        IVportalLogger<PushNotificationHubContext> logger,
        IObjectMapper objectMapper,
        IPushNotificationAppHubContext notificationHub)
    {
      this.logger = logger;
      this.objectMapper = objectMapper;
      this.notificationHub = notificationHub;
    }

    public async Task PushNotification(List<Guid> targetUsers, PushNotificationValueObject notification)
    {
      try
      {
        var mapped = objectMapper.Map<PushNotificationValueObject, PushNotificationDto>(notification);
        await notificationHub.NotifyUser(targetUsers, mapped);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }
  }
}
