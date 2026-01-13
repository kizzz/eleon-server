namespace Common.EventBus.Module.Interception
{
  public interface IEventSendInterceptor
  {
    Task Intercept(Type eventType, object eventData);
  }
}
