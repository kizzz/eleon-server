using Common.EventBus.Module;
using Common.Module.Constants;
using Eleon.Common.Lib.UserToken;
using EventBus.MassTransit.Module;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
  public class MassTransitEventBusResolveContributor : ITransientDependency, IDistributedBusResolveContributor
  {
    private readonly IServiceProvider serviceProvider;

    public EventBusProvider ProviderType => EventBusProvider.MassTransit;

    public MassTransitEventBusResolveContributor(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public async Task<IDistributedEventBus> Connect(EventBusOptions options)
    {
      var instance = new MassTransitDistributedEventBus(
          serviceProvider.GetRequiredService<IServiceScopeFactory>(),
          serviceProvider.GetRequiredService<IOptions<AbpDistributedEventBusOptions>>(),
          serviceProvider.GetRequiredService<ICurrentTenant>(),
          serviceProvider.GetRequiredService<IUnitOfWorkManager>(),
          serviceProvider.GetRequiredService<IGuidGenerator>(),
          serviceProvider.GetRequiredService<IClock>(),
          serviceProvider.GetRequiredService<IEventHandlerInvoker>(),
          serviceProvider.GetRequiredService<ILocalEventBus>(),
          serviceProvider.GetRequiredService<ICorrelationIdProvider>(),
          serviceProvider.GetRequiredService<ResponseContext>(),
          serviceProvider.GetRequiredService<EventContextManager>());

      return instance;
    }
  }
}
