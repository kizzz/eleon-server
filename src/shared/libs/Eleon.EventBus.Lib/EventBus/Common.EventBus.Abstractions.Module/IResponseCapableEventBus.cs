namespace Common.EventBus.Module
{
  public interface IResponseCapableEventBus
  {
    public Task<ResponseType> RequestAsync<ResponseType>(object eventData, int timeoutSeconds = 180)
        where ResponseType : class;
  }
}
