using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.Identity.Module.EventServices
{
  public class InitializeMicroserviceEventService :
      IDistributedEventHandler<InitializeMicroserviceMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<InitializeMicroserviceEventService> logger;
    private readonly MicroserviceManager microserviceDomainService;

    public InitializeMicroserviceEventService(
        IVportalLogger<InitializeMicroserviceEventService> logger,
        MicroserviceManager microserviceDomainService)
    {
      this.logger = logger;
      this.microserviceDomainService = microserviceDomainService;
    }

    public async Task HandleEventAsync(InitializeMicroserviceMsg eventData)
    {
      try
      {
        var info = eventData.Info;
        await microserviceDomainService.InitializeMicroservice(eventData.RequestId, info.ServiceId, info.DisplayName, info.Features);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }
  }
}


