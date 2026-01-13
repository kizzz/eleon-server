using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Lifecycle.Feature.Module.DomainServices;

namespace VPortal.Lifecycle.Feature.Module.EventServices
{
  public class CancelLifecycleEventService :
      IDistributedEventHandler<CancelLifecycleMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<CancelLifecycleEventService> logger;
    private readonly StatesGroupAuditDomainService statesGroupAuditDomainService;

    public CancelLifecycleEventService(
        IVportalLogger<CancelLifecycleEventService> logger,
        StatesGroupAuditDomainService statesGroupAuditDomainService)
    {
      this.logger = logger;
      this.statesGroupAuditDomainService = statesGroupAuditDomainService;
    }

    public async Task HandleEventAsync(CancelLifecycleMsg eventData)
    {
      try
      {
        await statesGroupAuditDomainService.DeepCancel(
            eventData.DocumentObjectType,
            eventData.DocumentId);
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
