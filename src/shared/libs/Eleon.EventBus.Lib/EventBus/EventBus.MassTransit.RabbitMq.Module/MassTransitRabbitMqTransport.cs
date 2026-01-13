using MassTransit;
using VPortal.MassTransit.RabbitMQ;

namespace EventBus.MassTransit.Module
{
  public class MassTransitRabbitMqTransport : IMassTransitTransport
  {
    private readonly MassTransitRabbitMqOptions options;

    public MassTransitRabbitMqTransport(MassTransitRabbitMqOptions options)
    {
      this.options = options;
    }

    public void ConfigureBus(IBusRegistrationConfigurator cfg)
    {
      cfg.UsingRabbitMq((context, rabbitMq) =>
      {
        rabbitMq.Host(options.Host, options.Port, options.VirtualHost, h =>
              {
            h.Username(options.Username);
            h.Password(options.Password);
            if (options.UseSsl)
            {
              throw new NotImplementedException();
            }

            if (options.UseCluster && options.ClusterNodes.Any())
            {
              h.UseCluster(c =>
                    {
                    foreach (var clusterNode in options.ClusterNodes)
                    {
                      c.Node(clusterNode);
                    }
                  });
            }
          });

        rabbitMq.UseDelayedMessageScheduler();
        rabbitMq.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(options.DefaultQueuePrefix));
      });
    }
  }
}
