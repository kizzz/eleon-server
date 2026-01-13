using Logging.Module;
using Messaging.Module.ETO;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.Services;

namespace VPortal.Notificator.Module.Auditor
{
  public class AuditorHubContext : IAuditorHubContext, ITransientDependency
  {
    private readonly IVportalLogger<AuditorHubContext> logger;
    private readonly IAuditorAppHubContext hubContext;

    public AuditorHubContext(
        IVportalLogger<AuditorHubContext> logger,
        IAuditorAppHubContext hubContext)
    {
      this.logger = logger;
      this.hubContext = hubContext;
    }

    public async Task NotifyVersionChanged(AuditVersionChangeNotificationEto notification)
    {
      try
      {
        await hubContext.NotifyVersionChanged(notification);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }
  }
}
