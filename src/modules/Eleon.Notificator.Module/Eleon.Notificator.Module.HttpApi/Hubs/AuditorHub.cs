using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using VPortal.Notificator.Module.HubGroups;

namespace VPortal.Notificator.Module.Hubs;

[Authorize]
[HubRoute("hubs/auditor/auditor-hub")]
public class AuditorHub : AbpHub
{
  private readonly IVportalLogger<AuditorHub> logger;

  public AuditorHub(IVportalLogger<AuditorHub> logger)
  {
    this.logger = logger;
  }

  public override async Task OnConnectedAsync()
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, new AuditorTenantGroupId(CurrentTenant).ToString());
    await base.OnConnectedAsync();
  }
}
