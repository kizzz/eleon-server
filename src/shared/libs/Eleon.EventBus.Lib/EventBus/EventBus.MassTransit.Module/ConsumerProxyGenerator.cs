using Castle.DynamicProxy;
using MassTransit;

namespace EventBus.MassTransit.Module
{
  public class ConsumerProxyGenerator : ProxyGenerator
  {
    private static Type StubConsumerType = typeof(StubConsumer);

    public Type CreateConsumerType(Type eventType, ProxyGenerationOptions options)
    {
      var consumerInterface = typeof(IConsumer<>).MakeGenericType(eventType);
      return CreateClassProxyType(StubConsumerType, [consumerInterface], options);
    }

    public object ActivateConsumerType(Type consumerType, ProxyGenerationOptions options, params IInterceptor[] interceptors)
    {
      List<object> list = BuildArgumentListForClassProxy(options, interceptors);
      return CreateClassProxyInstance(consumerType, list, StubConsumerType, []);
    }
  }
}
