using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Identity;

namespace ModuleCollector.Notificator.Module.Notificator.Module.HttpApi.Hubs;

public class IdentityAppHubContext : IIdentityAppHubContext
{
  private readonly IVportalLogger<IdentityAppHubContext> _logger;
  private readonly IHubContext<IdentityHub> _hubContext;

  public IdentityAppHubContext(
      IVportalLogger<IdentityAppHubContext> logger,
      IHubContext<IdentityHub> hubContext)
  {
    _logger = logger;
    _hubContext = hubContext;
  }

  public async Task CheckSessionAsync(Guid userId, object data)
  {
    try
    {
      var client = _hubContext.Clients.User(userId.ToString());

      if (client != null)
      {
        await client.SendAsync("CheckSession", data);
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
