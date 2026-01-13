using Eleon.Logging.Lib.VportalLogging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace Common.EventBus.Module;

[ExposeServices(typeof(IEventHandlerInvoker))]
[Dependency(ReplaceServices = true)]
public sealed class BoundaryLoggingEventHandlerInvoker : IEventHandlerInvoker, ISingletonDependency
{
  private readonly EventHandlerInvoker _inner;
  private readonly IBoundaryLogger _boundaryLogger;
  private readonly IExceptionReporter _exceptionReporter;

  public BoundaryLoggingEventHandlerInvoker(
      EventHandlerInvoker inner,
      IBoundaryLogger boundaryLogger,
      IExceptionReporter exceptionReporter)
  {
    _inner = inner;
    _boundaryLogger = boundaryLogger;
    _exceptionReporter = exceptionReporter;
  }

  public async Task InvokeAsync(IEventHandler eventHandler, object eventData, Type eventType)
  {
    if (!IsDistributedHandler(eventHandler, eventType))
    {
      await _inner.InvokeAsync(eventHandler, eventData, eventType).ConfigureAwait(false);
      return;
    }

    using var scope = _boundaryLogger.BeginConsumer(eventData);
    try
    {
      await _inner.InvokeAsync(eventHandler, eventData, eventType).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      _exceptionReporter.Report(ex, scope.Context);
      throw;
    }
  }

  private static bool IsDistributedHandler(IEventHandler eventHandler, Type eventType)
  {
    var distributedHandlerType = typeof(IDistributedEventHandler<>).MakeGenericType(eventType);
    return distributedHandlerType.IsInstanceOfType(eventHandler);
  }
}
