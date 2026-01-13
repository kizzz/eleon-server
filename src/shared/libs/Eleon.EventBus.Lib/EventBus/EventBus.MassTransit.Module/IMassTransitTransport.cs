using MassTransit;

namespace EventBus.MassTransit.Module
{
  public interface IMassTransitTransport
  {
    void ConfigureBus(IBusRegistrationConfigurator cfg);
  }
}
