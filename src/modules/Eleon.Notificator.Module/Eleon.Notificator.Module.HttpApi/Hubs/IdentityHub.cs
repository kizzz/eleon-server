using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using VPortal.Notificator.Module.Hubs;

namespace ModuleCollector.Notificator.Module.Notificator.Module.HttpApi.Hubs;

[Authorize]
[HubRoute("hubs/identity/identity-hub")]
public class IdentityHub : AbpHub
{
  private readonly IVportalLogger<PushNotificationHub> logger;

  public IdentityHub(IVportalLogger<PushNotificationHub> logger)
  {
    this.logger = logger;
  }
}
