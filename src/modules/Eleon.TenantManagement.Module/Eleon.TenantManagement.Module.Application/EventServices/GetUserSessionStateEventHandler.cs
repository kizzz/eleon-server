using Common.EventBus.Module;
using Commons.Module.Messages.SessionState;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Identity.Module.Sessions;

namespace VPortal.Identity.Module.EventServices;
public class GetUserSessionStateEventHandler :
    IDistributedEventHandler<GetUserSessionStateRequestMsg>,
    ITransientDependency
{
  private readonly UserSessionManager _userSessionManager;
  private readonly IResponseContext _responseContext;
  private readonly IVportalLogger<GetUserSessionStateEventHandler> _logger;

  public GetUserSessionStateEventHandler(
      UserSessionManager userSessionManager,
      IResponseContext responseContext,
      IVportalLogger<GetUserSessionStateEventHandler> logger)
  {
    _userSessionManager = userSessionManager;
    _responseContext = responseContext;
    _logger = logger;
  }

  public async Task HandleEventAsync(GetUserSessionStateRequestMsg eventData)
  {
    try
    {
      var response = await _userSessionManager.GetSessionState(eventData.UserId);

      await _responseContext.RespondAsync(new UserSessionStateResponseMsg
      {
        UserId = response.UserId,
        RequirePeriodicPasswordChange = response.RequirePeriodicPasswordChange,
        PermissionErrorEncountered = response.PermissionErrorEncountered,
      });
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }
}
