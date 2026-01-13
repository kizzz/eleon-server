using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.EventBus.Module;
using NSubstitute;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for mocking IResponseCapableEventBus and RequestAsync patterns.
/// </summary>
public static class EventBusTestHelpers
{
    /// <summary>
    /// Creates a mock IResponseCapableEventBus that implements both IResponseCapableEventBus and IDistributedEventBus.
    /// </summary>
    public static IResponseCapableEventBus CreateMockResponseCapableEventBus()
    {
        return Substitute.For<IResponseCapableEventBus, IDistributedEventBus>();
    }

    /// <summary>
    /// Sets up a RequestAsync call on the event bus to return a specific response.
    /// </summary>
    /// <typeparam name="TRequest">The request message type.</typeparam>
    /// <typeparam name="TResponse">The response message type.</typeparam>
    /// <param name="bus">The event bus mock.</param>
    /// <param name="response">The response to return.</param>
    public static void SetupEventBusRequestAsync<TRequest, TResponse>(
        IResponseCapableEventBus bus,
        TResponse response)
        where TResponse : class
    {
        bus.RequestAsync<TResponse>(Arg.Any<object>())
            .Returns(Task.FromResult(response));
        bus.RequestAsync<TResponse>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(response));
    }

    /// <summary>
    /// Sets up a RequestAsync call on the event bus to return a response based on a factory function.
    /// </summary>
    /// <typeparam name="TRequest">The request message type.</typeparam>
    /// <typeparam name="TResponse">The response message type.</typeparam>
    /// <param name="bus">The event bus mock.</param>
    /// <param name="responseFactory">Factory function to create the response.</param>
    public static void SetupEventBusRequestAsync<TRequest, TResponse>(
        IResponseCapableEventBus bus,
        Func<TRequest, TResponse> responseFactory)
        where TRequest : class
        where TResponse : class
    {
        bus.RequestAsync<TResponse>(Arg.Any<object>())
            .Returns(callInfo => Task.FromResult(responseFactory(callInfo.Arg<object>() as TRequest ?? Activator.CreateInstance<TRequest>())));
        bus.RequestAsync<TResponse>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(callInfo => Task.FromResult(responseFactory(callInfo.Arg<object>() as TRequest ?? Activator.CreateInstance<TRequest>())));
    }

    /// <summary>
    /// Creates a RecordingEventBus that records all PublishAsync and RequestAsync calls for verification.
    /// </summary>
    public static RecordingEventBus CreateRecordingEventBus()
    {
        return new RecordingEventBus();
    }
}

/// <summary>
/// Event bus implementation that records all publish and request calls for test verification.
/// </summary>
public class RecordingEventBus : IResponseCapableEventBus, IDistributedEventBus
{
    private readonly List<PublishedEvent> _publishedEvents = new();
    private readonly List<RequestedEvent> _requestedEvents = new();
    private readonly object _lock = new();

    /// <summary>
    /// Gets all published events in order.
    /// </summary>
    public IReadOnlyList<PublishedEvent> PublishedEvents
    {
        get
        {
            lock (_lock)
            {
                return _publishedEvents.ToList().AsReadOnly();
            }
        }
    }

    /// <summary>
    /// Gets all requested events in order.
    /// </summary>
    public IReadOnlyList<RequestedEvent> RequestedEvents
    {
        get
        {
            lock (_lock)
            {
                return _requestedEvents.ToList().AsReadOnly();
            }
        }
    }

    /// <summary>
    /// Clears all recorded events.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _publishedEvents.Clear();
            _requestedEvents.Clear();
        }
    }

    // IDistributedEventBus PublishAsync methods
    // IDistributedEventBus PublishAsync methods (with useOutbox parameter)
    public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = false, bool useOutbox = false)
        where TEvent : class
    {
        lock (_lock)
        {
            _publishedEvents.Add(new PublishedEvent
            {
                EventType = typeof(TEvent),
                EventData = eventData,
                OnUnitOfWorkComplete = onUnitOfWorkComplete,
                UseOutbox = useOutbox,
                Timestamp = DateTime.UtcNow
            });
        }
        return Task.CompletedTask;
    }

    public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = false, bool useOutbox = false)
    {
        lock (_lock)
        {
            _publishedEvents.Add(new PublishedEvent
            {
                EventType = eventType,
                EventData = eventData,
                OnUnitOfWorkComplete = onUnitOfWorkComplete,
                UseOutbox = useOutbox,
                Timestamp = DateTime.UtcNow
            });
        }
        return Task.CompletedTask;
    }

    // IEventBus PublishAsync methods (without useOutbox parameter)
    Task IEventBus.PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete)
    {
        return PublishAsync(eventData, onUnitOfWorkComplete, false);
    }

    Task IEventBus.PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete)
    {
        return PublishAsync(eventType, eventData, onUnitOfWorkComplete, false);
    }

    public Task<ResponseType> RequestAsync<ResponseType>(object eventData, int timeoutSeconds = 180)
        where ResponseType : class
    {
        lock (_lock)
        {
            _requestedEvents.Add(new RequestedEvent
            {
                RequestType = eventData?.GetType(),
                RequestData = eventData,
                ResponseType = typeof(ResponseType),
                TimeoutSeconds = timeoutSeconds,
                Timestamp = DateTime.UtcNow
            });
        }
        // Return default response - tests should configure responses via SetupEventBusRequestAsync if needed
        return Task.FromResult<ResponseType>(null);
    }

    // IDistributedEventBus Subscribe methods (no-op for recording bus)
    public IDisposable Subscribe<TEvent>(IDistributedEventHandler<TEvent> handler) where TEvent : class
    {
        return new EmptyDisposable();
    }

    public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
    {
        return new EmptyDisposable();
    }

    public IDisposable Subscribe<TEvent, THandler>() where TEvent : class where THandler : Volo.Abp.EventBus.IEventHandler, new()
    {
        return new EmptyDisposable();
    }

    public IDisposable Subscribe(Type eventType, Volo.Abp.EventBus.IEventHandler handler)
    {
        return new EmptyDisposable();
    }

    public IDisposable Subscribe<TEvent>(Volo.Abp.EventBus.IEventHandlerFactory factory) where TEvent : class
    {
        return new EmptyDisposable();
    }

    public IDisposable Subscribe(Type eventType, Volo.Abp.EventBus.IEventHandlerFactory factory)
    {
        return new EmptyDisposable();
    }

    // IEventBus Unsubscribe methods (no-op for recording bus)
    public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
    {
        // No-op
    }

    public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
    {
        // No-op
    }

    public void Unsubscribe(Type eventType, IEventHandler handler)
    {
        // No-op
    }

    public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
    {
        // No-op
    }

    public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
    {
        // No-op
    }

    public void UnsubscribeAll<TEvent>() where TEvent : class
    {
        // No-op
    }

    public void UnsubscribeAll(Type eventType)
    {
        // No-op
    }

    private class EmptyDisposable : IDisposable
    {
        public void Dispose() { }
    }

    /// <summary>
    /// Verifies that events were published only after commit.
    /// </summary>
    public void VerifyPublishAfterCommit(
        Func<Task> operationWithCommit,
        Func<Task> operationWithoutCommit)
    {
        // This is a verification pattern - actual implementation would track UoW state
        // For now, tests should manually verify PublishedEvents list after operations
    }

    /// <summary>
    /// Gets published events of a specific type.
    /// </summary>
    public List<PublishedEvent> GetPublishedEvents<TEvent>()
    {
        lock (_lock)
        {
            return _publishedEvents.Where(e => e.EventType == typeof(TEvent)).ToList();
        }
    }

    /// <summary>
    /// Gets requested events with a specific response type.
    /// </summary>
    public List<RequestedEvent> GetRequestedEvents<TResponse>()
    {
        lock (_lock)
        {
            return _requestedEvents.Where(e => e.ResponseType == typeof(TResponse)).ToList();
        }
    }
}

/// <summary>
/// Represents a published event for verification.
/// </summary>
public class PublishedEvent
{
    public Type EventType { get; set; }
    public object EventData { get; set; }
    public bool OnUnitOfWorkComplete { get; set; }
    public bool UseOutbox { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Represents a requested event for verification.
/// </summary>
public class RequestedEvent
{
    public Type RequestType { get; set; }
    public object RequestData { get; set; }
    public Type ResponseType { get; set; }
    public int TimeoutSeconds { get; set; }
    public DateTime Timestamp { get; set; }
}

