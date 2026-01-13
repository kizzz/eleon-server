namespace Common.EventBus.Module
{
  public interface IResponseContext
  {
    public Task RespondAsync(object data);
  }
}
