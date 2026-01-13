using Common.EventBus.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
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
  public class EleoncoreLocalEventBus : LocalDistributedEventBus, IResponseCapableEventBus
  {
    private readonly ILocalEventBus _localEventBus;
    private readonly EventContextManager _eventContextManager;
    private readonly ResponseContext _responseContext;

    // correlation-id → TCS map
    private readonly ConcurrentDictionary<string, TaskCompletionSource<object>> _responseHandlers;

    public EleoncoreLocalEventBus(
        IServiceScopeFactory serviceScopeFactory,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        IOptions<AbpDistributedEventBusOptions> distributedEventBusOptions,
        IGuidGenerator guidGenerator,
        IClock clock,
        IEventHandlerInvoker eventHandlerInvoker,
        ILocalEventBus localEventBus,
        ICorrelationIdProvider correlationIdProvider,
        EventContextManager eventContextManager,
        ResponseContext responseContext)
        : base(  // 👈  **exact same order as the base class**
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
      _localEventBus = localEventBus;
      _eventContextManager = eventContextManager;
      _responseContext = responseContext;
      _responseHandlers = new ConcurrentDictionary<string, TaskCompletionSource<object>>();
    }

    public void Initialize()
    {
    }

    public async Task<ResponseType> RequestAsync<ResponseType>(object eventData, int timeoutSeconds = 180)
        where ResponseType : class
    {
      var correlationId = Guid.NewGuid().ToString();
      var responseTaskSource = new TaskCompletionSource<object>();

      // Register the response handler
      if (!_responseHandlers.TryAdd(correlationId, responseTaskSource))
      {
        throw new InvalidOperationException("Failed to register response handler.");
      }

      try
      {
        // Create and attach the event context
        var eventContextJson = _eventContextManager.CreateEventContext();

        // Wrap the event context and set the correlation ID
        _eventContextManager.UnwrapEventContext(eventContextJson);

        // Set the response context before publishing
        var responder = new InMemoryEventBusResponder(responseTaskSource);
        _responseContext.SetContext(responder);

        // Publish the event data
        await PublishAsync(eventData.GetType(), eventData, false, false);

        // Set up a timeout
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
        var completedTask = await Task.WhenAny(timeoutTask, responseTaskSource.Task);

        if (completedTask == timeoutTask)
        {
          throw new TimeoutException($"Request timed out after {timeoutSeconds} seconds.");
        }

        // Return the response if available
        return responseTaskSource.Task.Result as ResponseType;
      }
      finally
      {
        // Clean up the response handler to avoid memory leaks
        _responseHandlers.TryRemove(correlationId, out _);
        _responseContext.SetContext(null); // Clear response context after completion
      }
    }

    public async Task HandleResponseAsync(string correlationId, object response)
    {
      // Complete the response handler if the correlation ID matches
      if (_responseHandlers.TryGetValue(correlationId, out var responseTaskSource))
      {
        responseTaskSource.SetResult(response);
      }
    }
  }

  // Responder implementation specific to in-memory event bus
  public class InMemoryEventBusResponder : IDistributedEventBusResponder
  {
    private readonly TaskCompletionSource<object> _responseTaskSource;

    public InMemoryEventBusResponder(TaskCompletionSource<object> responseTaskSource)
    {
      _responseTaskSource = responseTaskSource;
    }

    public Task RespondAsync(object data)
    {
      _responseTaskSource.SetResult(data);
      return Task.CompletedTask;
    }
  }
}
