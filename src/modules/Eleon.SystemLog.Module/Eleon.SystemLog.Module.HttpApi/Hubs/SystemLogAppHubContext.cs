
using Logging.Module;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using VPortal.DocMessageLog.Module.DocMessageLogs;

namespace VPortal.SystemLog.Module.Hubs;

[ExposeServices(typeof(ISystemLogAppHubContext))]
public class SystemLogAppHubContext : ISystemLogAppHubContext, ITransientDependency
{
  private readonly IVportalLogger<SystemLogAppHubContext> _logger;

  private readonly IHubContext<SystemLogHub> _hubContext;

  public SystemLogAppHubContext(
      IVportalLogger<SystemLogAppHubContext> logger,
      IHubContext<SystemLogHub> hubContext)
  {
    _logger = logger;
    _hubContext = hubContext;
  }

  public async Task PushSystemLogAsync(List<Guid> targetUsers, SystemLogDto logDto)
  {
    try
    {
      var client = _hubContext.Clients.Users(targetUsers.Select(x => x.ToString()));
      if (client != null)
      {
        await client.SendAsync("PushSystemLog", logDto);
      }
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }
}
