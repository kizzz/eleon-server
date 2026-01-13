using MassTransit;

namespace EventBus.MassTransit.Module
{
  public class MassTransitInMemoryTransport : IMassTransitTransport
  {
    public void ConfigureBus(IBusRegistrationConfigurator cfg)
    {
      cfg.UsingInMemory((context, c) =>
      {
        c.ConfigureEndpoints(context);
      });
    }
  }
}
