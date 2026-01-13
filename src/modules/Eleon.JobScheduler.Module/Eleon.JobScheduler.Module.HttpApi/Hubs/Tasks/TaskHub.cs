using JobScheduler.Module.HubGroups;
using JobScheduler.Module.Tasks;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Migrations.Module;
using Volo.Abp.AspNetCore.SignalR;

namespace VPortal.JobScheduler.Module.Tasks
{
  [Authorize]
  [HubRoute("hubs/JobScheduler/TaskHub")]
  public class TaskHub : AbpHub<ITaskAppHubContext>
  {
    private readonly IVportalLogger<TaskHub> logger;

    public TaskHub(IVportalLogger<TaskHub> logger)
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
