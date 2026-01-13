namespace Common.EventBus.Module
{
  public interface IDistributedEventBusResponder
  {
    Task RespondAsync(object eventData);
  }
}
