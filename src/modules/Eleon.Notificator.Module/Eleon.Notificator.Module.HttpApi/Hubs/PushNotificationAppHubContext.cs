using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.PushNotifications;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.NotificationLogs;
using VPortal.Notificator.Module.PushNotifications;

namespace VPortal.Notificator.Module.Hubs
{
  public class PushNotificationAppHubContext : IPushNotificationAppHubContext, ITransientDependency
  {
    private readonly IVportalLogger<PushNotificationAppHubContext> logger;
    private readonly IHubContext<PushNotificationHub> hubContext;

    public PushNotificationAppHubContext(
        IVportalLogger<PushNotificationAppHubContext> logger,
        IHubContext<PushNotificationHub> hubContext)
    {
      this.logger = logger;
      this.hubContext = hubContext;
    }

    [AllowAnonymous]
    public async Task NotifyUser(List<Guid> userIds, PushNotificationDto notification)
    {
      try
      {
        var clients = hubContext.Clients.Users(userIds.Select(x => x.ToString()));
        if (clients != null)
        {
          await clients.SendAsync("NotifyUser", notification);
        }
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
