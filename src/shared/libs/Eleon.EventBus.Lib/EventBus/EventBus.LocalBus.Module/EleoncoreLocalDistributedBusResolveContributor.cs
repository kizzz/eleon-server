using Common.EventBus.Module;
using Common.EventBus.Module.Options;
using Common.Module.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace EventBus.Nats
{
  [ExposeServices(typeof(IDistributedBusResolveContributor))]
  public class EleoncoreLocalDistributedBusResolveContributor : ITransientDependency, IDistributedBusResolveContributor
  {
    private readonly IServiceProvider serviceProvider;

    public EleoncoreLocalDistributedBusResolveContributor(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public EventBusProvider ProviderType => EventBusProvider.InMemory;

    public async Task<IDistributedEventBus> Connect(EventBusOptions options)
    {
      var providerOptions = JsonConvert.DeserializeObject<NatsOptions>(options.ProviderOptionsJson);

      var instance = new EleoncoreLocalEventBus(
          serviceProvider.GetRequiredService<IServiceScopeFactory>(),
          serviceProvider.GetRequiredService<ICurrentTenant>(),
          serviceProvider.GetRequiredService<IUnitOfWorkManager>(),
          serviceProvider.GetRequiredService<IOptions<AbpDistributedEventBusOptions>>(),
          serviceProvider.GetRequiredService<IGuidGenerator>(),
          serviceProvider.GetRequiredService<IClock>(),              // ← clock
          serviceProvider.GetRequiredService<IEventHandlerInvoker>(),
          serviceProvider.GetRequiredService<ILocalEventBus>(),
          serviceProvider.GetRequiredService<ICorrelationIdProvider>(),
          serviceProvider.GetRequiredService<EventContextManager>(),
          serviceProvider.GetRequiredService<ResponseContext>()
      );

      // Initialize the event bus instance
      instance.Initialize();

      return instance;
    }
  }
}
