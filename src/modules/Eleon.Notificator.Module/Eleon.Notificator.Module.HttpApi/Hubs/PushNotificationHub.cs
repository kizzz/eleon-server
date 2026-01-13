using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;

namespace VPortal.Notificator.Module.Hubs
{
  [Authorize]
  [HubRoute("hubs/notificator/push-notifications-hub")]
  public class PushNotificationHub : AbpHub
  {
    private readonly IVportalLogger<PushNotificationHub> logger;

    public PushNotificationHub(IVportalLogger<PushNotificationHub> logger)
    {
      this.logger = logger;
    }
  }
}
