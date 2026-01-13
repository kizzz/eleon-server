using Common.EventBus.Module;
using EventBus.MassTransit.Module;
using MassTransit;
using MassTransit.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
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
  public class MassTransitDistributedEventBus : DistributedEventBusBase, IResponseCapableEventBus
  {
    private readonly ResponseContext responseContext;
    private readonly EventContextManager eventContextManager;

    protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }
    protected ConcurrentDictionary<string, Type> EventTypes { get; }

    public MassTransitDistributedEventBus(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<AbpDistributedEventBusOptions> distributedEventBusOptions,
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
      this.responseContext = responseContext;
      this.eventContextManager = eventContextManager;
    }

    public static void Initialize(MassTransitDistributedEventBus bus)
    {
      bus.SubscribeHandlers(bus.AbpDistributedEventBusOptions.Handlers);
    }

    /// <inheritdoc/>
    public override IDisposable Subscribe(Type eventType, IEventHandlerFactory defaultFactory)
    {
      var handlerFactories = GetOrCreateHandlerFactories(eventType);

      IEventHandlerFactory factory = defaultFactory;

      if (factory.IsInFactories(handlerFactories))
      {
        return NullDisposable.Instance;
      }

      handlerFactories.Add(factory);

      MassTransitHostStore
        .GetOrInitialize(ServiceScopeFactory)
        .AddConsumer(eventType, ProcessEventAsync);

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
      MassTransitHostStore.GetOrInitialize(ServiceScopeFactory);
      using var scope = MassTransitHostStore.GetHost().CreateScope();
      var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
      await publishEndpoint.Publish(eventData, eventType, context =>
      {
        context.Headers.Set(
                  EventContextConsts.EventContextHeaderName,
                  eventContextManager.CreateEventContext());
      });
    }

    public async Task<ResponseType> RequestAsync<EventType, ResponseType>(EventType eventData)
        where EventType : class
        where ResponseType : class
    {
      MassTransitHostStore.GetOrInitialize(ServiceScopeFactory);
      using var scope = MassTransitHostStore.GetHost().CreateScope();
      var client = scope.ServiceProvider.GetRequiredService<IRequestClient<EventType>>();
      var response = await client.GetResponse<ResponseType>(
          eventData,
          cfg => cfg.UseInlineFilter(async (ctx, next) =>
          {
            ctx.Headers.Set(
                      EventContextConsts.EventContextHeaderName,
                      eventContextManager.CreateEventContext());

            await next.Send(ctx);
          }),
          timeout: RequestTimeout.After(m: 3));
      return response.Message;
    }

    public async Task<ResponseType> RequestAsync<ResponseType>(object eventData, int timeoutSeconds = 180)
        where ResponseType : class
    {
      MassTransitHostStore.GetOrInitialize(ServiceScopeFactory);
      using var scope = MassTransitHostStore.GetHost().CreateScope();

      var requestType = eventData.GetType();

      var clientType = typeof(IRequestClient<>).MakeGenericType(requestType);

      var configuratorCallbackType = typeof(RequestPipeConfiguratorCallback<>).MakeGenericType(requestType);

      Type[] getResponseArgTypes = [
          requestType,
                configuratorCallbackType,
                typeof(CancellationToken),
                typeof(RequestTimeout)];

      var getResponseOverloads = clientType
          .GetMethods()
          .Where(x => x.Name == "GetResponse")
          .Select(m => new
          {
            Method = m,
            Params = m.GetParameters(),
            Args = m.GetGenericArguments()
          });

      var getResponseOverloadWithConfig = getResponseOverloads
          .Where(x =>
              x.Args.Length == 1
              && x.Params.Length == getResponseArgTypes.Length
              && x.Params.Select((p, ix) => p.ParameterType == getResponseArgTypes[ix]).All(p => p))
          .FirstOrDefault();

      var getResponseMethod = getResponseOverloadWithConfig
          .Method
          .GetGenericMethodDefinition()
          .MakeGenericMethod([typeof(ResponseType)]);
      if (getResponseMethod == null)
      {
        throw new InvalidOperationException("GetResponse method not found.");
      }

      var configurator = Activator.CreateInstance(
          typeof(SendConfigurator<>).MakeGenericType(requestType),
          eventContextManager.CreateEventContext());
      var cfgDelegate = Delegate.CreateDelegate(configuratorCallbackType, configurator, "Configure");

      var client = scope.ServiceProvider.GetRequiredService(clientType);
      var responseTask = (Task)getResponseMethod.Invoke(
          client,
          [
              eventData,
                    cfgDelegate,
                    default(CancellationToken),
                    RequestTimeout.After(s: timeoutSeconds),
          ]);

      await responseTask.ConfigureAwait(false);
      var responseProperty = responseTask.GetType().GetProperty("Result");
      var response = (MessageResponse<ResponseType>)responseProperty?.GetValue(responseTask);

      return response.Message;
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

    private async Task ProcessEventAsync(MassTransitConsumeContext ctx)
    {
      if (eventContextManager.IsCurrentEventDuplicate())
      {
        return;
      }

      responseContext.SetContext(new MassTransitDistributedEventBusResponder(ctx.OriginalContext));
      await TriggerHandlersDirectAsync(ctx.EventType, ctx.EventData);
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

    private class SendConfigurator<T> where T : class
    {
      private readonly string eventContext;

      public SendConfigurator(string eventContext)
      {
        this.eventContext = eventContext;
      }

      public void Configure(IRequestPipeConfigurator<T> cfg)
      {
        cfg.UseInlineFilter(async (ctx, next) =>
        {
          ctx.Headers.Set(
                      EventContextConsts.EventContextHeaderName,
                      eventContext);

          await next.Send(ctx);
        });
      }
    }
  }
}
