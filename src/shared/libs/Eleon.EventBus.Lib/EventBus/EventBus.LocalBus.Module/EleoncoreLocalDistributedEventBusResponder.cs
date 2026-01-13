using Common.EventBus.Module;
using Newtonsoft.Json;

namespace EventBus.Nats
{
  internal class EleoncoreLocalDistributedEventBusResponder<T> : IDistributedEventBusResponder
  {

    public EleoncoreLocalDistributedEventBusResponder()
    {
    }

    public async Task RespondAsync(object eventData)
    {
      var payload = JsonConvert.SerializeObject(eventData);
    }
  }
}
