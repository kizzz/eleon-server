using Common.EventBus.Module;
using EventBus.RabbitMqOverrides.Module;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Volo.Abp;
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

namespace EventBus.RabbitMq.Module
{
  /// <summary>
  /// RabbitMQ-backed distributed event-bus that also supports request/response.
  /// Ported to RabbitMQ.Client 7.x (IChannel API).
  /// </summary>
  public class RabbitMqEventBus : RabbitMqDistributedEventBus, IResponseCapableEventBus
  {
    private readonly ResponseContext responseContext;
    private readonly EventContextManager eventContextManager;

    private new IRabbitMqMessageConsumer Consumer { get; set; }

    public RabbitMqEventBus(
        IOptions<AbpRabbitMqEventBusOptions> options,
        IConnectionPool connectionPool,
        IRabbitMqSerializer serializer,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<AbpDistributedEventBusOptions> distributedEventBusOptions,
        IRabbitMqMessageConsumerFactory messageConsumerFactory,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        IGuidGenerator guidGenerator,
        IClock clock,
        IEventHandlerInvoker eventHandlerInvoker,
        ILocalEventBus localEventBus,
        ICorrelationIdProvider correlationIdProvider,
        ResponseContext responseContext,
        EventContextManager eventContextManager)
        : base(
              options,
              connectionPool,
              serializer,
              serviceScopeFactory,
              distributedEventBusOptions,
              messageConsumerFactory,
              currentTenant,
              unitOfWorkManager,
              guidGenerator,
              clock,
              eventHandlerInvoker,
              localEventBus,
              correlationIdProvider)
    {
      this.responseContext = responseContext;
      this.eventContextManager = eventContextManager;
    }

    public new void Initialize()
    {
      Consumer = MessageConsumerFactory.Create(
          new ExchangeDeclareConfiguration(
              AbpRabbitMqEventBusOptions.ExchangeName,
              type: AbpRabbitMqEventBusOptions.GetExchangeTypeOrDefault(),
              durable: true
          ),
          new QueueDeclareConfiguration(
              AbpRabbitMqEventBusOptions.ClientName,
              durable: true,
              exclusive: false,
              autoDelete: false,
              prefetchCount: AbpRabbitMqEventBusOptions.PrefetchCount
          ),
          AbpRabbitMqEventBusOptions.ConnectionName
      );

      Consumer.OnMessageReceived(ProcessEventAsync);
      SubscribeHandlers(AbpDistributedEventBusOptions.Handlers);
    }

    /**********************************************************************
     *  1.  IModel  ->  IChannel
     *  2.  BasicDeliverEventArgs stays the same
     *  3.  No functional changes – purely mechanical
     *********************************************************************/
    private async Task ProcessEventAsync(IChannel channel, BasicDeliverEventArgs ea)
    {
      var eventName = ea.RoutingKey;
      var eventType = EventTypes.GetOrDefault(eventName);
      if (eventType == null)
      {
        return;
      }

      if (ea.BasicProperties.Headers.TryGetValue(EventContextConsts.EventContextHeaderName, out var contextHeader))
      {
        eventContextManager.UnwrapEventContext(contextHeader.ToString()!);
      }

      if (eventContextManager.IsCurrentEventDuplicate())
      {
        return;
      }

      var eventData = Serializer.Deserialize(ea.Body.ToArray(), eventType);

      var correlationId = ea.BasicProperties.CorrelationId;
      if (await AddToInboxAsync(ea.BasicProperties.MessageId, eventName, eventType, eventData, correlationId))
      {
        return;
      }

      await eventContextManager.RegisterMessageConsume(eventType, eventData);

      var responder = new RabbitMqDistributedEventBusResponder(channel, ea, Serializer);
      responseContext.SetContext(responder);

      using (CorrelationIdProvider.Change(correlationId))
      {
        await TriggerHandlersDirectAsync(eventType, eventData);
      }
    }

    public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
    {
      var handlerFactories = GetOrCreateHandlerFactories(eventType);

      if (factory.IsInFactories(handlerFactories))
      {
        return NullDisposable.Instance;
      }

      handlerFactories.Add(factory);

      if (handlerFactories.Count == 1) //TODO: Multi-threading!
      {
        Consumer.BindAsync(EventNameAttribute.GetNameOrDefault(eventType));
      }

      return new EventHandlerFactoryUnregistrar(this, eventType, factory);
    }

    protected override async Task PublishToEventBusAsync(Type eventType, object eventData)
    {
      var headers = new Dictionary<string, object>
      {
        [EventContextConsts.EventContextHeaderName] = eventContextManager.CreateEventContext()
      };

      await PublishAsync(eventType, eventData, headers, correlationId: CorrelationIdProvider.Get());
    }

    public async Task<ResponseType> RequestAsync<ResponseType>(object eventData, int timeoutSeconds = 180)
where ResponseType : class
    {
      // 1️⃣  Get the actual IConnection instance
      var connection = await ConnectionPool
          .GetAsync(AbpRabbitMqEventBusOptions.ConnectionName);

      // 2️⃣  Open a channel (IChannel) on that connection
      await using var channel = await connection.CreateChannelAsync();

      var responseTcs = new TaskCompletionSource<ResponseType>();

      var consumer = new AsyncEventingBasicConsumer(channel);
      consumer.ReceivedAsync += async (_, response) =>
      {
        var payload = Serializer.Deserialize<ResponseType>(response.Body.ToArray());
        responseTcs.TrySetResult(payload);
      };

      await channel.BasicConsumeAsync(
          queue: RabbitMqReplyConsts.ReplyQueueName,
          autoAck: true,
          consumer: consumer);

      var properties = new BasicProperties
      {
        DeliveryMode = DeliveryModes.Persistent,
        MessageId = GuidGenerator.Create().ToString("N"),
        ReplyTo = RabbitMqReplyConsts.ReplyQueueName,
        Headers = new Dictionary<string, object>
        {
          [EventContextConsts.EventContextHeaderName] =
                  eventContextManager.CreateEventContext()
        }
      };

      var body = Serializer.Serialize(eventData);

      await channel.BasicPublishAsync(
          exchange: AbpRabbitMqEventBusOptions.ExchangeName,
          routingKey: EventNameAttribute.GetNameOrDefault(eventData.GetType()),
          mandatory: true,
          basicProperties: properties,
          body: body);

      var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
      var completed = await Task.WhenAny(timeoutTask, responseTcs.Task);

      if (completed == timeoutTask)
        throw new TimeoutException(
            $"RabbitMQ request timed out after {timeoutSeconds} seconds.");

      return await responseTcs.Task.ConfigureAwait(false);
    }

    /**********************************************************************
     * Helpers – unchanged except for generic type rename
     *********************************************************************/
    private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
    {
      return HandlerFactories.GetOrAdd(
          eventType,
          type =>
          {
            var eventName = EventNameAttribute.GetNameOrDefault(type);
            EventTypes.GetOrAdd(eventName, eventType);
            return new List<IEventHandlerFactory>();
          });
    }
  }
}
