using Common.EventBus.Module.Interception;
using Eleon.Common.Lib.UserToken;
using Logging.Module;
using Messaging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace Common.EventBus.Module;

//[ExposeServices(typeof(IDistributedEventBus))]
//[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
[Obsolete("Use SystemDistributedEventBus instead")]
public class CommonDistributedEventBus : IDistributedEventBus, ITransientDependency, IResponseCapableEventBus
{
  private readonly DistributedBusResolver resolver;
  private readonly SystemEventMessageManager systemEventMessageManager;
  private readonly CurrentEvent currentEvent;
  private readonly IGuidGenerator guidGenerator;
  private readonly IEnumerable<IEventSendInterceptor> interceptors;
  private readonly ILogger<CommonDistributedEventBus> logger;
  private readonly IConfiguration configuration;
  private readonly IUnitOfWorkManager _unitOfWorkManager;
  private readonly ICurrentTenant _currentTenant;

  public CommonDistributedEventBus(
      DistributedBusResolver resolver,
      SystemEventMessageManager systemEventMessageManager,
      CurrentEvent currentEvent,
      IGuidGenerator guidGenerator,
      IEnumerable<IEventSendInterceptor> interceptors,
      ILogger<CommonDistributedEventBus> logger,
      IConfiguration configuration,
      IUnitOfWorkManager unitOfWorkManager,
      ICurrentTenant currentTenant)
  {
    this.resolver = resolver;
    this.systemEventMessageManager = systemEventMessageManager;
    this.currentEvent = currentEvent;
    this.guidGenerator = guidGenerator;
    this.interceptors = interceptors;
    this.logger = logger;
    this.configuration = configuration;
    _unitOfWorkManager = unitOfWorkManager;
    _currentTenant = currentTenant;
  }

  private bool IsSystemMessagesEnabled()
  {
    var result = configuration["SystemMessages:Enabled"];

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
      if (eventType == typeof(EntityCreatedEto<EntityEto>) ||
          eventType == typeof(EntityUpdatedEto<EntityEto>) ||
          eventType == typeof(EntityDeletedEto<EntityEto>))
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
        catch (Exception)
        {
        }
      }
    }

    return false;
  }

  public async Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true)
  {
    if (IsSystemMessagesEnabled() && eventType != typeof(SystemEventMsg) && IsSystemEventType(eventType, eventData))
    {
      try
      {
        var uow = _unitOfWorkManager.Current;
        var tenantId = _currentTenant.Id;

        if (onUnitOfWorkComplete && uow != null && !uow.IsCompleted)
        {
          var key = $"SysEvent:{eventType.FullName}:{RuntimeHelpers.GetHashCode(eventData)}";
          if (!uow.Items.ContainsKey(key))
          {
            uow.Items[key] = true;
            uow.OnCompleted(async () =>
            {
              using (_currentTenant.Change(tenantId))
              {
                var sysMsg = systemEventMessageManager.GenerateMessage(eventData);
                await PublishAsync(typeof(SystemEventMsg), sysMsg, onUnitOfWorkComplete: false, useOutbox).ConfigureAwait(false);
              }
            });
          }
        }
        else
        {
          using (_currentTenant.Change(tenantId))
          {
            var sysMsg = systemEventMessageManager.GenerateMessage(eventData);
            await PublishAsync(typeof(SystemEventMsg), sysMsg, onUnitOfWorkComplete: false, useOutbox).ConfigureAwait(false);
          }
        }
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Error while publishing system event for {EventType}", eventType);
      }
    }

    RegisterCurrentEvent(eventType, eventData);
    await InterceptCurrentEvent(eventType, eventData);
    await ForAllBuses(b =>
    {
      return b.PublishAsync(eventType, eventData, onUnitOfWorkComplete, useOutbox);
    });
  }

  public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true) where TEvent : class
  {
    return PublishAsync(typeof(TEvent), eventData, onUnitOfWorkComplete, useOutbox);
  }

  public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true) where TEvent : class
  {
    return PublishAsync(typeof(TEvent), eventData, onUnitOfWorkComplete);
  }

  public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true)
  {
    return PublishAsync(eventType, eventData, onUnitOfWorkComplete);
  }

  public async Task<ResponseType> RequestAsync<ResponseType>(object eventData, int timeoutSeconds = 180)
      where ResponseType : class
  {
    RegisterCurrentEvent(eventData.GetType(), eventData);
    await InterceptCurrentEvent(eventData.GetType(), eventData);
    var buses = await resolver.ResolveTenantEventBuses();
    var tasks = buses.Select(b => b.RequestAsync<ResponseType>(eventData, timeoutSeconds)).ToList();
    var exceptions = new List<TimeoutException>();

    while (tasks.Count > 0)
    {
      Task<ResponseType> firstToComplete = null;
      try
      {
        firstToComplete = await Task.WhenAny(tasks);
        return firstToComplete.Result;
      }
      catch (TimeoutException timeout)
      {
        tasks.Remove(firstToComplete);
        exceptions.Add(timeout);
      }
    }

    throw new TimeoutException("All buses timed out on request. See inner exceptions.", exceptions.First());
  }

  public IDisposable Subscribe<TEvent>(IDistributedEventHandler<TEvent> handler) where TEvent : class
      => ForeachBusSync(b => b.Subscribe(handler));

  public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
      => ForeachBusSync(b => b.Subscribe(action));

  public IDisposable Subscribe<TEvent, THandler>()
      where TEvent : class
      where THandler : IEventHandler, new()
      => ForeachBusSync(b => b.Subscribe<TEvent, THandler>());

  public IDisposable Subscribe(Type eventType, IEventHandler handler)
      => ForeachBusSync(b => b.Subscribe(eventType, handler));

  public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
      => ForeachBusSync(b => b.Subscribe<TEvent>(factory));

  public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
      => ForeachBusSync(b => b.Subscribe(eventType, factory));

  public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
      => ForeachBusSync(b => b.Unsubscribe(action));

  public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
      => ForeachBusSync(b => b.Unsubscribe(handler));

  public void Unsubscribe(Type eventType, IEventHandler handler)
      => ForeachBusSync(b => b.Unsubscribe(eventType, handler));

  public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
      => ForeachBusSync(b => b.Unsubscribe<TEvent>(factory));

  public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
      => ForeachBusSync(b => b.Unsubscribe(eventType, factory));

  public void UnsubscribeAll<TEvent>() where TEvent : class
      => ForeachBusSync(b => b.UnsubscribeAll<TEvent>());

  public void UnsubscribeAll(Type eventType)
      => ForeachBusSync(b => b.UnsubscribeAll(eventType));

  private async Task ForAllBuses(Func<IDistributedEventBus, Task> action)
      => await Task.WhenAll((await resolver.ResolveTenantEventBuses()).Select(x => action(x)));

  private void ForeachBusSync(Action<IDistributedEventBus> action)
      => resolver.ResolveTenantEventBuses().GetAwaiter().GetResult().ForEach(action);

  private CombinedDisposable ForeachBusSync(Func<IDistributedEventBus, IDisposable> action)
      => new(resolver.ResolveTenantEventBuses().GetAwaiter().GetResult().Select(x => action(x)));

  private void RegisterCurrentEvent(Type eventType, object eventData)
  {
    currentEvent.EventId = guidGenerator.Create();
    currentEvent.EventSendTime = DateTime.UtcNow;
  }

  private async Task InterceptCurrentEvent(Type eventType, object eventData)
  {
    foreach (var interceptor in interceptors)
    {
      await interceptor.Intercept(eventType, eventData);
    }
  }

  private class CombinedDisposable : List<IDisposable>, IDisposable
  {
    public CombinedDisposable(IEnumerable<IDisposable> items) : base(items)
    {

    }

    public void Dispose()
    {
      foreach (var item in this)
      {
        item.Dispose();
      }
    }
  }
}
