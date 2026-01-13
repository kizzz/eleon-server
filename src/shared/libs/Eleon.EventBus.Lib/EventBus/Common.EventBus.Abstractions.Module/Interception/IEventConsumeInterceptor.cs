namespace Common.EventBus.Module.Interception
{
  public interface IEventConsumeInterceptor
  {
    Task Intercept(Type eventType, object eventData);
  }
}
