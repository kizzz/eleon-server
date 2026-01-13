using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Migrations.Module;
using Volo.Abp.AspNetCore.SignalR;
using VPortal.DocMessageLog.Module.DocMessageLogs;

namespace VPortal.SystemLog.Module.Hubs
{
  [Authorize]
  [HubRoute("hubs/SystemLog/SystemLog")]
  public class SystemLogHub : AbpHub
  {
    private readonly IVportalLogger<SystemLogHub> logger;

    public SystemLogHub(IVportalLogger<SystemLogHub> logger)
    {
      this.logger = logger;
    }
  }
}
