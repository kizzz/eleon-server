using Common.EventBus.Module;
using Common.EventBus.Module.Options;
using Common.Module.Constants;
using Logging.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
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
  public class NatsEventBusResolveContributor : ITransientDependency, IDistributedBusResolveContributor
  {
    private readonly IServiceProvider serviceProvider;

    public NatsEventBusResolveContributor(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public EventBusProvider ProviderType => EventBusProvider.NATS;

    public async Task<IDistributedEventBus> Connect(EventBusOptions options)
    {
      var providerOptions = JsonConvert.DeserializeObject<NatsOptions>(options.ProviderOptionsJson);

      var connection = new NatsConnectionWrapper(
          serviceProvider.GetRequiredService<ILogger<NatsConnectionWrapper>>(),
          serviceProvider.GetRequiredService<ResponseContext>(),
          serviceProvider.GetRequiredService<EventContextManager>(),
          NatsOpts.Default with
          {
            Url = providerOptions.Url,
          });

      var instance = new NatsDistributedEventBus(
          serviceProvider.GetRequiredService<IServiceScopeFactory>(),
          serviceProvider.GetRequiredService<IOptions<AbpDistributedEventBusOptions>>(),
          serviceProvider.GetRequiredService<ICurrentTenant>(),
          serviceProvider.GetRequiredService<IUnitOfWorkManager>(),
          serviceProvider.GetRequiredService<IGuidGenerator>(),
          serviceProvider.GetRequiredService<IClock>(),
          serviceProvider.GetRequiredService<IEventHandlerInvoker>(),
          serviceProvider.GetRequiredService<ILocalEventBus>(),
          serviceProvider.GetRequiredService<ICorrelationIdProvider>(),
          connection,
          serviceProvider.GetRequiredService<EventContextManager>());

      instance.Initialize();

      return instance;
    }
  }
}
