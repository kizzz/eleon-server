using EventBus.MassTransit.Module;
using Volo.Abp.DependencyInjection;

namespace EventBus.MassTransit.RabbitMq.Module
{
  [ExposeServices(typeof(IMassTransitTransportResolveContributor))]
  public class RabbitMqTransportResolveContributor : IMassTransitTransportResolveContributor, ITransientDependency
  {
    public void Resolve(MassTransitTransportResolveContext context)
    {
      if (context.Options.Type != MassTransitOptions.TransportType.RabbitMQ)
      {
        return;
      }

      if (context.Options.RabbitMqOptions == null)
      {
        throw new Exception("MassTransit Rabbit MQ transport is not configured.");
      }

      context.Transport = new MassTransitRabbitMqTransport(context.Options.RabbitMqOptions);
      context.Resolved = true;
    }
  }
}
