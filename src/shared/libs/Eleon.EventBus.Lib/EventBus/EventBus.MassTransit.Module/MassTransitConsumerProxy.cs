using Castle.DynamicProxy;
using Common.EventBus.Module;
using MassTransit;

namespace EventBus.MassTransit.Module
{
  internal class MassTransitConsumerProxy : IInterceptor
  {
    private readonly Type eventType;
    private readonly Func<MassTransitConsumeContext, Task> consumer;
    private readonly EventContextManager eventContextManager;

    public MassTransitConsumerProxy(
        Type eventType,
        Func<MassTransitConsumeContext, Task> consumer,
        EventContextManager eventContextManager)
    {
      this.eventType = eventType;
      this.consumer = consumer;
      this.eventContextManager = eventContextManager;
    }

    public async void Intercept(IInvocation invocation)
    {
      var consumeContext = invocation.GetArgumentValue(0) as ConsumeContext;
      var msgPropInfo = consumeContext.GetType().GetProperty(nameof(ConsumeContext<object>.Message));
      var msg = msgPropInfo.GetValue(consumeContext);

      if (consumeContext.Headers.TryGetHeader(EventContextConsts.EventContextHeaderName, out var contextJson))
      {
        eventContextManager.UnwrapEventContext(contextJson.ToString()!);
      }

      //await eventContextManager.RegisterMessageConsume(eventType, msg);

      invocation.ReturnValue = consumer(new MassTransitConsumeContext()
      {
        OriginalContext = consumeContext,
        EventData = msg,
        EventType = eventType,
      });
    }
  }

  public class MassTransitConsumeContext
  {
    public required ConsumeContext OriginalContext { get; init; }
    public required Type EventType { get; set; }
    public required object EventData { get; init; }
  }

  public class StubConsumer
  {

  }
}
