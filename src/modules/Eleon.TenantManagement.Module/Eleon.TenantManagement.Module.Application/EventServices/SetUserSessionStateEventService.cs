using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Identity.Module.Sessions;

namespace VPortal.Identity.Module.EventServices
{
  public class SetUserSessionStateEventService :
      IDistributedEventHandler<SetUserSessionStateMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<SetUserSessionStateEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly UserSessionManager sessionManager;

    public SetUserSessionStateEventService(
        IVportalLogger<SetUserSessionStateEventService> logger,
        IResponseContext responseContext,
        UserSessionManager sessionManager)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.sessionManager = sessionManager;
    }

    public async Task HandleEventAsync(SetUserSessionStateMsg eventData)
    {
      var msg = eventData;
      var response = new UserSessionStateSetMsg();
      try
      {
        await sessionManager.SetSessionState(msg.UserId, msg.RequirePeriodicPasswordChange, msg.PermissionErrorEncountered);
      }
      catch (Exception e)
      {
        logger.CaptureAndSuppress(e);
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
