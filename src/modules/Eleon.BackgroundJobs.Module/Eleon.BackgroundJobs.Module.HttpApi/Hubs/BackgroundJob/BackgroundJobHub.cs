using BackgroundJobs.Module.BackgroundJobs;
using BackgroundJobs.Module.HubGroups;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Migrations.Module;
using Volo.Abp.AspNetCore.SignalR;

namespace Eleon.BackgroundJobs.Module.Eleon.BackgroundJobs.Module.HttpApi.Hubs.BackgroundJob
{
  [Authorize]
  [HubRoute("hubs/BackgroundJob/BackgroundJobHub")]
  public class BackgroundJobHub : AbpHub<IBackgroundJobAppHubContext>
  {
    private readonly IVportalLogger<BackgroundJobHub> logger;

    public BackgroundJobHub(IVportalLogger<BackgroundJobHub> logger)
    {
      this.logger = logger;
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
}
