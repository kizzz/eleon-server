using Common.EventBus.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Volo.Abp;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace EventBus.Nats
{
  public class NatsDistributedEventBus : DistributedEventBusBase, IResponseCapableEventBus
  {
    private readonly NatsConnectionWrapper natsConnectionWrapper;
    private readonly EventContextManager eventContextManager;

    protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }
    protected ConcurrentDictionary<string, Type> EventTypes { get; }

    public NatsDistributedEventBus(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<AbpDistributedEventBusOptions> distributedEventBusOptions,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        IGuidGenerator guidGenerator,
        IClock clock,
        IEventHandlerInvoker eventHandlerInvoker,
        ILocalEventBus localEventBus,
        ICorrelationIdProvider correlationIdProvider,
        NatsConnectionWrapper natsConnectionWrapper,
        EventContextManager eventContextManager)
        : base(
            serviceScopeFactory,
            currentTenant,
            unitOfWorkManager,
            distributedEventBusOptions,
            guidGenerator,
            clock,
            eventHandlerInvoker,
            localEventBus,
            correlationIdProvider)
    {
      HandlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
      EventTypes = new ConcurrentDictionary<string, Type>();
      this.natsConnectionWrapper = natsConnectionWrapper;
      this.eventContextManager = eventContextManager;
    }

    public void Initialize()
    {
      SubscribeHandlers(AbpDistributedEventBusOptions.Handlers);
      natsConnectionWrapper.StartListening($"*", ProcessEventAsync);
    }

    /// <inheritdoc/>
    public override IDisposable Subscribe(Type eventType, IEventHandlerFactory defaultFactory)
    {
      var handlerFactories = GetOrCreateHandlerFactories(eventType);

      IEventHandlerFactory factory = defaultFactory;
      //if (factory is IocEventHandlerFactory iocFactory)
      //{
      //    factory = new IocEventHandlerFactory(responseScopeContextFactory, iocFactory.HandlerType);
      //}

      if (factory.IsInFactories(handlerFactories))
      {
        return NullDisposable.Instance;
      }

      handlerFactories.Add(factory);

      return new EventHandlerFactoryUnregistrar(this, eventType, factory);
    }

    /// <inheritdoc/>
    public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
    {
      Check.NotNull(action, nameof(action));

      GetOrCreateHandlerFactories(typeof(TEvent))
          .Locking(factories =>
          {
            factories.RemoveAll(
                      factory =>
                      {
                    var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                    if (singleInstanceFactory == null)
                    {
                      return false;
                    }

                    var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEvent>;
                    if (actionHandler == null)
                    {
                      return false;
                    }

                    return actionHandler.Action == action;
                  });
          });
    }

    /// <inheritdoc/>
    public override void Unsubscribe(Type eventType, IEventHandler handler)
    {
      GetOrCreateHandlerFactories(eventType)
          .Locking(factories =>
          {
            factories.RemoveAll(
                      factory =>
                          factory is SingleInstanceHandlerFactory &&
                          (factory as SingleInstanceHandlerFactory)!.HandlerInstance == handler
                  );
          });
    }

    /// <inheritdoc/>
    public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
    {
      GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
    }

    /// <inheritdoc/>
    public override void UnsubscribeAll(Type eventType)
    {
      GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
    }

    protected async override Task PublishToEventBusAsync(Type eventType, object eventData)
    {
      await using var conn = natsConnectionWrapper.GetConnection();
      string subjName = EventNameAttribute.GetNameOrDefault(eventType);
      var payload = JsonConvert.SerializeObject(eventData);
      var headers = new NatsHeaders()
      {
        [EventContextConsts.EventContextHeaderName] = eventContextManager.CreateEventContext(),
      };

      await conn.PublishAsync(subjName, payload, headers);
    }

    public async Task<ResponseType> RequestAsync<ResponseType>(object eventData, int timeoutSeconds = 180)
        where ResponseType : class
    {
      await using var conn = natsConnectionWrapper.GetConnection();
      string subjName = EventNameAttribute.GetNameOrDefault(eventData.GetType());
      var payload = JsonConvert.SerializeObject(eventData);
      var headers = new NatsHeaders()
      {
        [EventContextConsts.EventContextHeaderName] = eventContextManager.CreateEventContext(),
      };

      var response = await conn.RequestAsync<string, string>(subjName, payload, headers, replyOpts: new NatsSubOpts()
      {
        Timeout = TimeSpan.FromSeconds(timeoutSeconds),
      });

      return JsonConvert.DeserializeObject<ResponseType>(response.Data);
    }

    protected override void AddToUnitOfWork(IUnitOfWork unitOfWork, UnitOfWorkEventRecord eventRecord)
    {
      unitOfWork.AddOrReplaceDistributedEvent(eventRecord);
    }

    public async override Task PublishFromOutboxAsync(
        OutgoingEventInfo outgoingEvent,
        OutboxConfig outboxConfig)
    {
      throw new NotImplementedException();
    }

    public async override Task PublishManyFromOutboxAsync(
        IEnumerable<OutgoingEventInfo> outgoingEvents,
        OutboxConfig outboxConfig)
    {
      throw new NotImplementedException();
    }

    public async override Task ProcessFromInboxAsync(
        IncomingEventInfo incomingEvent,
        InboxConfig inboxConfig)
    {
      throw new NotImplementedException();
    }

    protected override byte[] Serialize(object eventData)
    {
      throw new NotImplementedException();
    }

    protected override Task OnAddToOutboxAsync(string eventName, Type eventType, object eventData)
    {
      EventTypes.GetOrAdd(eventName, eventType);
      return base.OnAddToOutboxAsync(eventName, eventType, eventData);
    }

    private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
    {
      return HandlerFactories.GetOrAdd(
          eventType,
          type =>
          {
            var eventName = EventNameAttribute.GetNameOrDefault(type);
            EventTypes.GetOrAdd(eventName, eventType);
            return new List<IEventHandlerFactory>();
          }
      );
    }

    protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
    {
      var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

      foreach (var handlerFactory in
               HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
      {
        handlerFactoryList.Add(
            new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
      }

      return handlerFactoryList.ToArray();
    }

    private async Task ProcessEventAsync(string subject, string data)
    {
      var eventType = EventTypes.GetOrDefault(subject);
      if (eventType == null)
      {
        return;
      }

      var eventData = JsonConvert.DeserializeObject(data, eventType);

      await TriggerHandlersDirectAsync(eventType, eventData);
    }

    private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
    {
      //Should trigger same type
      if (handlerEventType == targetEventType)
      {
        return true;
      }

      //TODO: Support inheritance? But it does not support on subscription to RabbitMq!
      //Should trigger for inherited types
      if (handlerEventType.IsAssignableFrom(targetEventType))
      {
        return true;
      }

      return false;
    }
  }
}
