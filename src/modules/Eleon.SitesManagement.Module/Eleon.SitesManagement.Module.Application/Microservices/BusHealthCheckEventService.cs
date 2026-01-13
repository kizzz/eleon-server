using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace VPortal.SitesManagement.Module.Microservices
{
  public class BusHealthCheckEventService :
      IDistributedEventHandler<ScheduleMsg>,
      ITransientDependency
  {
    private readonly MicroserviceManager microserviceDomainService;
    private readonly IConfiguration configuration;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly IVportalLogger<BusHealthCheckEventService> logger;

    public BusHealthCheckEventService(
        MicroserviceManager microserviceDomainService,
        IConfiguration configuration,
        IDistributedEventBus massTransitPublisher,
        IVportalLogger<BusHealthCheckEventService> logger)
    {
      this.microserviceDomainService = microserviceDomainService;
      this.configuration = configuration;
      this.massTransitPublisher = massTransitPublisher;
      this.logger = logger;
    }

    public async Task HandleEventAsync(ScheduleMsg eventData)
    {
      try
      {


        if (configuration.GetValue<bool>("IsHealthcheck") != false)
        {
          var (checkBusIds, checkHttpIds) = await microserviceDomainService.GetServiceIdsToHealthCheck();
          await RequestBusCheck(checkBusIds);

        }
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }

    private async Task RequestBusCheck(List<Guid> serviceIds)
    {
      if (serviceIds.IsNullOrEmpty())
      {
        return;
      }

      var request = new RequestBusHealthCheckMsg();
      request.ServiceIds = serviceIds;
      await massTransitPublisher.PublishAsync(request);
    }
  }
}


