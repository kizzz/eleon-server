using Volo.Abp.DependencyInjection;

namespace Common.EventBus.Module
{
  public class ResponseContext : IResponseContext, ITransientDependency
  {
    private static readonly AsyncLocal<IDistributedEventBusResponder?> Responder = new(null);

    public void SetContext(IDistributedEventBusResponder? responder)
    {
      Responder.Value = responder;
    }

    public void MapContextTo(ResponseContext mapTo)
    {
      mapTo.SetContext(Responder.Value);
    }

    public async Task RespondAsync(object data)
    {
      if (Responder.Value == null)
      {
        throw new Exception("Unable to respond to message outside of a response context.");
      }

      await Responder.Value.RespondAsync(data);
    }
  }
}
