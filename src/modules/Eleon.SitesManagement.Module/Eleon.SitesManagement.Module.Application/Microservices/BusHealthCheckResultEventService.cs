using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.SitesManagement.Module.Microservices
{
  public class BusHealthCheckResultEventService :
      IDistributedEventHandler<BusHealthCheckedMsg>,
      ITransientDependency
  {
    private readonly MicroserviceManager microserviceDomainService;
    private readonly IVportalLogger<BusHealthCheckResultEventService> logger;

    public BusHealthCheckResultEventService(
        MicroserviceManager microserviceDomainService,
        IVportalLogger<BusHealthCheckResultEventService> logger)
    {
      this.microserviceDomainService = microserviceDomainService;
      this.logger = logger;
    }

    public async Task HandleEventAsync(BusHealthCheckedMsg eventData)
    {
      try
      {
        await microserviceDomainService.SetBusHealthChecked(eventData.ServiceId);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }
  }
}


