using Common.EventBus.Module;
using MassTransit;

namespace EventBus.Nats
{
  internal class MassTransitDistributedEventBusResponder : IDistributedEventBusResponder
  {
    private readonly ConsumeContext consumeContext;

    public MassTransitDistributedEventBusResponder(ConsumeContext consumeContext)
    {
      this.consumeContext = consumeContext;
    }

    public async Task RespondAsync(object eventData)
    {
      await consumeContext.RespondAsync(eventData);
    }
  }
}
