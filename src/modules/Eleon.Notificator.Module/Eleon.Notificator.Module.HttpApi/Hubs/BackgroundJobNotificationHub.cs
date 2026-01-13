using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Migrations.Module;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;
using VPortal.Notificator.Module.BackgroundJobs;
using VPortal.Notificator.Module.HubGroups;

namespace VPortal.Notificator.Module.Hubs;

[Authorize]
[HubRoute("hubs/background-jobs/background-job-notification-hub")]
public class BackgroundJobNotificationHub : AbpHub<IBackgroundJobNotificationClient>
{
  private readonly IVportalLogger<BackgroundJobNotificationHub> _logger;

  public BackgroundJobNotificationHub(
      IVportalLogger<BackgroundJobNotificationHub> logger)
  {
    _logger = logger;
  }

  public override async Task OnConnectedAsync()
  {
    if (CurrentUser.IsInRole(MigrationConsts.AdminRoleNameDefaultValue))
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, new TenantAdminGroupId(CurrentTenant).ToString());
    }

    await Groups.AddToGroupAsync(Context.ConnectionId, new TenantGroupId(CurrentTenant).ToString());
    await base.OnConnectedAsync();
  }
}
