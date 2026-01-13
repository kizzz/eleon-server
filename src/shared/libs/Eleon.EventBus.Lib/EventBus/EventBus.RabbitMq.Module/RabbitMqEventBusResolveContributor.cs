using Common.EventBus.Module;
using Common.EventBus.Module.Options;
using Common.Module.Constants;
using EventBus.RabbitMq.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.RabbitMQ;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace EventBus.Nats
{
  [ExposeServices(typeof(IDistributedBusResolveContributor))]
  public class RabbitMqEventBusResolveContributor : ITransientDependency, IDistributedBusResolveContributor
  {
    private readonly IServiceProvider serviceProvider;
    private readonly RabbitMqDynamicConnections connections;
    private readonly IGuidGenerator guidGenerator;

    public EventBusProvider ProviderType => EventBusProvider.RabbitMQ;

    public RabbitMqEventBusResolveContributor(IServiceProvider serviceProvider, RabbitMqDynamicConnections connections, IGuidGenerator guidGenerator)
    {
      this.serviceProvider = serviceProvider;
      this.connections = connections;
      this.guidGenerator = guidGenerator;
    }

    public async Task<IDistributedEventBus> Connect(EventBusOptions options)
    {
      var providerOptions = JsonConvert.DeserializeObject<RabbitMqOptions>(options.ProviderOptionsJson);

      var instanceId = guidGenerator.Create();
      string connectionName = $"rabbit-connection-{instanceId}";
      connections[connectionName] = CreateConnection();

      var abpOptions = new AbpRabbitMqEventBusOptions()
      {
        ClientName = "EleoncoreDistributedBus",
        ExchangeName = "EleoncoreDistributedBus",
        ConnectionName = connectionName,
      };

      var instance = new RabbitMqEventBus(
          new StubOptions<AbpRabbitMqEventBusOptions>(abpOptions),
          Resolve<IConnectionPool>(),
          Resolve<IRabbitMqSerializer>(),
          Resolve<IServiceScopeFactory>(),
          Resolve<IOptions<AbpDistributedEventBusOptions>>(),
          Resolve<IRabbitMqMessageConsumerFactory>(),
          Resolve<ICurrentTenant>(),
          Resolve<IUnitOfWorkManager>(),
          Resolve<IGuidGenerator>(),
          Resolve<IClock>(),
          Resolve<IEventHandlerInvoker>(),
          Resolve<ILocalEventBus>(),
          Resolve<ICorrelationIdProvider>(),
          Resolve<ResponseContext>(),
          Resolve<EventContextManager>());

      instance.Initialize();

      return instance;
    }

    private ConnectionFactory CreateConnection()
    {
      var f = new ConnectionFactory();
      f.ConsumerDispatchConcurrency = 2;
      return f;
    }

    private T Resolve<T>() where T : notnull => serviceProvider.GetRequiredService<T>();

    private class StubOptions<T>(T value) : IOptions<T> where T : class
    {
      public T Value => value;
    }
  }
}
