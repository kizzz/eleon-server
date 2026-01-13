using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using VPortal.Lifecycle.Feature.Module.DomainServices;

namespace VPortal.Lifecycle.Feature.Module.EventServices
{

  public class StartLifecycleEventService :
      IDistributedEventHandler<StartLifecycleMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<StartLifecycleEventService> logger;
    private readonly LifecycleManagerDomainService lifecycleManagerDomainService;
    private readonly IResponseContext responseContext;
    private readonly ICurrentTenant currentTenant;

    public StartLifecycleEventService(
        IVportalLogger<StartLifecycleEventService> logger,
        LifecycleManagerDomainService lifecycleManagerDomainService,
        IResponseContext responseContext,
        ICurrentTenant currentTenant
    )
    {
      this.logger = logger;
      this.lifecycleManagerDomainService = lifecycleManagerDomainService;
      this.responseContext = responseContext;
      this.currentTenant = currentTenant;
    }

    public async Task HandleEventAsync(StartLifecycleMsg eventData)
    {
      var response = new ActionCompletedMsg();
      try
      {
        await lifecycleManagerDomainService.StartExistingLifecycle(
            eventData.DocumentObjectType,
            eventData.DocumentId);
        response.Success = true;
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
