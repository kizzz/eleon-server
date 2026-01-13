using Commons.Module.Messages.Session;
using Logging.Module;
using ModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Identity;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Application.EventHandlers;
public class SessionRevokedEventHandler : IDistributedEventHandler<UserSessionsRevokedMsg>, ITransientDependency
{
  private readonly IIdentityAppHubContext _identityAppHubContext;
  private readonly IVportalLogger<SessionRevokedEventHandler> _logger;

  public SessionRevokedEventHandler(IIdentityAppHubContext identityAppHubContext, IVportalLogger<SessionRevokedEventHandler> logger)
  {
    _identityAppHubContext = identityAppHubContext;
    _logger = logger;
  }

  public async Task HandleEventAsync(UserSessionsRevokedMsg eventData)
  {
    try
    {
      await _identityAppHubContext.CheckSessionAsync(eventData.UserId, null);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex, "Error while handling UserSessionsRevokedMsg event");
    }
    finally
    {
    }
  }
}
