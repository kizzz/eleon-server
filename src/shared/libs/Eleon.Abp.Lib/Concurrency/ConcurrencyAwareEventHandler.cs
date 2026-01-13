using Logging.Module;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace SharedModule.modules.Helpers.Module.EventHandlers;

/// <summary>
/// Base class for distributed event handlers that provides automatic concurrency exception handling.
/// Domain services should handle concurrency gracefully, but this base class ensures event handlers
/// don't fail on "already completed" scenarios.
/// </summary>
/// <typeparam name="TEvent">The event type</typeparam>
/// <typeparam name="THandler">The handler type (for logger)</typeparam>
public abstract class ConcurrencyAwareEventHandler<TEvent, THandler> : IDistributedEventHandler<TEvent>, ITransientDependency
    where THandler : class
{
  protected readonly IVportalLogger<THandler> Logger;

  protected ConcurrencyAwareEventHandler(IVportalLogger<THandler> logger)
  {
    Logger = logger;
  }

  /// <summary>
  /// Implement this method to handle the event. This will be wrapped with concurrency exception handling.
  /// </summary>
  /// <param name="eventData">The event data</param>
  protected abstract Task HandleEventInternalAsync(TEvent eventData);

  /// <summary>
  /// Handles the event with automatic concurrency exception handling.
  /// </summary>
  /// <param name="eventData">The event data</param>
  public async Task HandleEventAsync(TEvent eventData)
  {
    try
    {
      await HandleEventInternalAsync(eventData);
    }
    catch (AbpDbConcurrencyException ex)
    {
      // Domain service should handle this gracefully, but log for monitoring
      Logger.Log.LogWarning(ex,
          "Concurrency exception in event handler {EventHandlerType} for event {EventType}, but domain service should have handled it.",
          GetType().Name,
          typeof(TEvent).Name);
      // Don't rethrow - domain service treats "already completed" as success
    }
    catch (Exception e)
    {
      Logger.Capture(e);
      // Re-throw non-concurrency exceptions for retry/dead-letter handling
      throw;
    }
    finally
    {
    }
  }
}

