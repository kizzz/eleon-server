using Logging.Module;
using Messaging.Module.Messages;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Authorization.Module.MicroserviceInitialization
{
  public class TriggerMicroserviceInitializationEventService :
      IDistributedEventHandler<TriggerMicroserviceInitializationMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<TriggerMicroserviceInitializationEventService> logger;
    private readonly MicroserviceInitializer initializer;

    public TriggerMicroserviceInitializationEventService(
        IVportalLogger<TriggerMicroserviceInitializationEventService> logger,
        MicroserviceInitializer initializer)
    {
      this.logger = logger;
      this.initializer = initializer;
    }

    public async Task HandleEventAsync(TriggerMicroserviceInitializationMsg eventData)
    {
      try
      {
        await initializer.InitializeMicroservice(eventData.RequestId);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

    }
  }
}
