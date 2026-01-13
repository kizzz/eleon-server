using Commons.Module.Messages.Session;
using Identity.Module.Application.Contracts.IdentityServerServices;
using Logging.Module;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EventBus.Distributed;
using VPortal.Identity.Module.Sessions;

namespace Identity.Module.Application.IdentityServerServices;
public class RemoveCurrentSessionLogoutHandler : ILogoutHandler
{
  private readonly SessionManager sessionsManager;
  private readonly IVportalLogger<RemoveCurrentSessionLogoutHandler> _logger;
  private readonly IDistributedEventBus _eventBus;

  public RemoveCurrentSessionLogoutHandler(
      SessionManager sessionsManager,
      IVportalLogger<RemoveCurrentSessionLogoutHandler> logger,
      IDistributedEventBus eventBus)
  {
    this.sessionsManager = sessionsManager;
    this._logger = logger;
    _eventBus = eventBus;
  }

  public async Task ExecuteAsync()
  {
    try
    {
      var session = await sessionsManager.GetCurrentAsync();
      await sessionsManager.RevokeAsync(session.Id);
      if (Guid.TryParse(session.UserId, out var userId))
      {
        await _eventBus.PublishAsync(
            new UserSessionsRevokedMsg
            {
              UserId = userId,
              SessionId = session.Id,
            });
      }
    }
    catch (EntityNotFoundException notFound)
    {
      _logger.Log.LogWarning(notFound, "Current session was not found");
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
