using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Notificator.Module.DomainServices;

namespace VPortal.Notificator.Module.EventServices
{
  public class AuditVersionChangedEventService :
      IDistributedEventHandler<AuditVersionChangeNotificationMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<AuditVersionChangedEventService> logger;
    private readonly AuditVersionChangeDomainService domainService;

    public AuditVersionChangedEventService(
        IVportalLogger<AuditVersionChangedEventService> logger,
        AuditVersionChangeDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task HandleEventAsync(AuditVersionChangeNotificationMsg eventData)
    {
      try
      {
        await domainService.NotifyVersionChanged(eventData.AuditChange);
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
