using Logging.Module;
using Messaging.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Authorization.Module.MicroserviceInitialization
{
  public class MicroserviceHealthCheckEventService :
      IDistributedEventHandler<RequestBusHealthCheckMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<MicroserviceHealthCheckEventService> logger;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly MicroserviceInitializer initializer;

    public MicroserviceHealthCheckEventService(
        IVportalLogger<MicroserviceHealthCheckEventService> logger,
        IDistributedEventBus massTransitPublisher,
        MicroserviceInitializer initializer)
    {
      this.logger = logger;
      this.massTransitPublisher = massTransitPublisher;
      this.initializer = initializer;
    }

    public async Task HandleEventAsync(RequestBusHealthCheckMsg eventData)
    {
      try
      {
        var serviceId = await initializer.GetServiceId();
        if (eventData.ServiceIds.Contains(serviceId))
        {
          var response = new BusHealthCheckedMsg();
          response.ServiceId = serviceId;
          await massTransitPublisher.PublishAsync(response);
        }
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }
  }
}
