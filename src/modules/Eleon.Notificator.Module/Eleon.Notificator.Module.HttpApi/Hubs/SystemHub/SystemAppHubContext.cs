using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.System;
using Logging.Module;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.HttpApi.Hubs.SystemHub;

public class SystemAppHubContext : ISystemAppHubContext, ITransientDependency
{
  private readonly IVportalLogger<SystemAppHubContext> _logger;
  private readonly IHubContext<SystemHub> _hubContext;

  public SystemAppHubContext(
      IVportalLogger<SystemAppHubContext> logger,
      IHubContext<SystemHub> hubContext)
  {
    _logger = logger;
    _hubContext = hubContext;
  }


  public async Task SendToAsync(string method, object data, List<Guid> userIds)
  {
    try
    {
      var client = _hubContext.Clients.Users(userIds.Select(x => x.ToString()));
      await client.SendAsync(method, data);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }

  public async Task SendToAllAsync(string method, object data)
  {
    try
    {
      var client = _hubContext.Clients.All;
      await client.SendAsync(method, data);
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
