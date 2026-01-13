using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Common.EventBus.Module.Interception;
using Eleon.Common.Lib.UserToken;
using Eleon.EventBus.Lib.Full.EventBus.Common.EventBus.Module;
using Logging.Module;
using Messaging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUglify.Helpers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace Common.EventBus.Module;

[ExposeServices(typeof(IDistributedEventBus))]
[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
public class SystemDistributedEventBus
    : IDistributedEventBus,
        ITransientDependency,
        IResponseCapableEventBus
{
    private readonly IEnumerable<IDistributedBusResolveContributor> _contributors;
    private readonly SystemEventMessageManager _systemEventMessageManager;
    private readonly CurrentEvent _currentEvent;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IEnumerable<IEventSendInterceptor> _interceptors;
    private readonly ILogger<SystemDistributedEventBus> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ICancellationTokenProvider _cancellationTokenProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<EventBusOptions> _eventBusOptions;

    public SystemDistributedEventBus(
        IEnumerable<IDistributedBusResolveContributor> contributors,
        SystemEventMessageManager systemEventMessageManager,
        CurrentEvent currentEvent,
        IGuidGenerator guidGenerator,
        IEnumerable<IEventSendInterceptor> interceptors,
        ILogger<SystemDistributedEventBus> logger,
        IConfiguration configuration,
        IUnitOfWorkManager unitOfWorkManager,
        ICancellationTokenProvider cancellationTokenProvider,
        IServiceProvider serviceProvider
    )
    {
        _contributors = contributors;
        this._systemEventMessageManager = systemEventMessageManager;
        this._currentEvent = currentEvent;
        this._guidGenerator = guidGenerator;
        this._interceptors = interceptors;
        this._logger = logger;
        this._configuration = configuration;
        _unitOfWorkManager = unitOfWorkManager;
        _cancellationTokenProvider = cancellationTokenProvider;
        _serviceProvider = serviceProvider;
        _eventBusOptions = _serviceProvider.GetRequiredService<IOptions<EventBusOptions>>();
    }

    private bool IsSystemMessagesEnabled()
    {
        var result = _configuration["SystemMessages:Enabled"];

        if (bool.TryParse(result, out var isEnabled) && isEnabled)
        {
            return true;
        }

        return false;
    }

    private static bool IsSystemEventType(Type eventType, object eventData)
    {
        if (MessagingConsts.SystemEventTypes.Contains(eventType))
        {
            return true;
        }

        if (eventType.IsGenericType)
        {
            if (
                eventType == typeof(EntityCreatedEto<EntityEto>)
                || eventType == typeof(EntityUpdatedEto<EntityEto>)
                || eventType == typeof(EntityDeletedEto<EntityEto>)
            )
            {
                try
                {
                    var entityProperty = eventType.GetProperty("Entity");
                    if (entityProperty != null)
                    {
                        var entityValue = (EntityEto)entityProperty.GetValue(eventData);
                        if (MessagingConsts.EntityEtoTypes.Contains(entityValue.EntityType))
                        {
                            return true;
                        }
                    }
                }
                catch (Exception) { }
            }
        }

        return false;
    }

  public async Task PublishAsync(
        Type eventType,
        object eventData,
        bool onUnitOfWorkComplete = true,
        bool useOutbox = true
    )
    {
        if (
            IsSystemMessagesEnabled()
            && eventType != typeof(SystemEventMsg)
            && IsSystemEventType(eventType, eventData)
        )
        {
            try
            {
                var uow = _unitOfWorkManager.Current;
                var sysMsg = _systemEventMessageManager.GenerateMessage(eventData);

                if (onUnitOfWorkComplete && uow != null && !uow.IsCompleted)
                {
                    uow.AddOrReplaceDistributedEvent(
                        new UnitOfWorkEventRecord(
                            typeof(SystemEventMsg),
                            sysMsg,
                            EventOrderGenerator.GetNext(),
                            useOutbox
                        )
                    );
                    uow.AddOrReplaceDistributedEvent(
                        new UnitOfWorkEventRecord(
                            eventType,
                            eventData,
                            EventOrderGenerator.GetNext(),
                            useOutbox
                        )
                    );
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while publishing system event for {EventType}",
                    eventType
                );
            }
        }

        if (BusHelpers.IsPreventedType(eventType))
        {
            return;
        }

        RegisterCurrentEvent(eventType, eventData);
        await InterceptCurrentEvent(eventType, eventData);
        await ForAllBuses(b =>
        {
            return b.PublishAsync(eventType, eventData, onUnitOfWorkComplete, useOutbox);
        });
    }

    public Task PublishAsync<TEvent>(
        TEvent eventData,
        bool onUnitOfWorkComplete = true,
        bool useOutbox = true
    )
        where TEvent : class
    {
        return PublishAsync(typeof(TEvent), eventData, onUnitOfWorkComplete, useOutbox);
    }

    public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true)
        where TEvent : class
    {
        return PublishAsync(typeof(TEvent), eventData, onUnitOfWorkComplete);
    }

    public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true)
    {
        return PublishAsync(eventType, eventData, onUnitOfWorkComplete);
    }

    public async Task<ResponseType> RequestAsync<ResponseType>(
        object eventData,
        int timeoutSeconds = 180
    )
        where ResponseType : class
    {
        RegisterCurrentEvent(eventData.GetType(), eventData);
        await InterceptCurrentEvent(eventData.GetType(), eventData);

        var bus = await GetEventBusAsync();
        var task = bus.RequestAsync<ResponseType>(eventData, timeoutSeconds);

        return await task.WaitAsync(_cancellationTokenProvider.Token);
    }

    public IDisposable Subscribe<TEvent>(IDistributedEventHandler<TEvent> handler)
        where TEvent : class => ForeachBusSync(b => b.Subscribe(handler));

    public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action)
        where TEvent : class => ForeachBusSync(b => b.Subscribe(action));

    public IDisposable Subscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler, new() =>
        ForeachBusSync(b => b.Subscribe<TEvent, THandler>());

    public IDisposable Subscribe(Type eventType, IEventHandler handler) =>
        ForeachBusSync(b => b.Subscribe(eventType, handler));

    public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory)
        where TEvent : class => ForeachBusSync(b => b.Subscribe<TEvent>(factory));

    public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory) =>
        ForeachBusSync(b => b.Subscribe(eventType, factory));

    public void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        where TEvent : class => ForAllBusesSync(b => b.Unsubscribe(action));

    public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler)
        where TEvent : class => ForAllBusesSync(b => b.Unsubscribe(handler));

    public void Unsubscribe(Type eventType, IEventHandler handler) =>
        ForAllBusesSync(b => b.Unsubscribe(eventType, handler));

    public void Unsubscribe<TEvent>(IEventHandlerFactory factory)
        where TEvent : class => ForAllBusesSync(b => b.Unsubscribe<TEvent>(factory));

    public void Unsubscribe(Type eventType, IEventHandlerFactory factory) =>
        ForAllBusesSync(b => b.Unsubscribe(eventType, factory));

    public void UnsubscribeAll<TEvent>()
        where TEvent : class => ForAllBusesSync(b => b.UnsubscribeAll<TEvent>());

    public void UnsubscribeAll(Type eventType) => ForAllBusesSync(b => b.UnsubscribeAll(eventType));

    private async Task ForAllBuses(Func<IDistributedEventBus, Task> action) =>
        await action(await GetEventBusAsync());

    private void ForAllBusesSync(Action<IDistributedEventBus> action) =>
        action(GetEventBusAsync().GetAwaiter().GetResult());

    private IDisposable ForeachBusSync(Func<IDistributedEventBus, IDisposable> action) =>
        action(GetEventBusAsync().GetAwaiter().GetResult());

    private void RegisterCurrentEvent(Type eventType, object eventData)
    {
        _currentEvent.EventId = _guidGenerator.Create();
        _currentEvent.EventSendTime = DateTime.UtcNow;
    }

    private async Task InterceptCurrentEvent(Type eventType, object eventData)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.Intercept(eventType, eventData);
        }
    }

    private Task<IDistributedEventBus> GetEventBusAsync()
    {
        var contributor = _contributors.FirstOrDefault(x =>
            x.ProviderType == _eventBusOptions.Value.Provider
        );

        if (contributor == null)
        {
            throw new Exception("Unable to resolve an event bus in the current context.");
        }

        var bus = contributor.Connect(_eventBusOptions.Value);

        return bus;
    }
}
