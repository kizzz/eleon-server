using Common.EventBus.Module;
using NATS.Client.Core;
using Newtonsoft.Json;

namespace EventBus.Nats
{
  internal class NatsDistributedEventBusResponder<T> : IDistributedEventBusResponder
  {
    private readonly INatsMsg<T> natsMsg;

    public NatsDistributedEventBusResponder(INatsMsg<T> natsMsg)
    {
      this.natsMsg = natsMsg;
    }

    public async Task RespondAsync(object eventData)
    {
      var payload = JsonConvert.SerializeObject(eventData);
      await natsMsg.ReplyAsync(payload);
    }
  }
}
