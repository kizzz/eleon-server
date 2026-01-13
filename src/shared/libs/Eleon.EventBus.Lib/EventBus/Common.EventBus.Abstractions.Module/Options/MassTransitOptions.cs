using VPortal.MassTransit.RabbitMQ;

namespace EventBus.MassTransit.Module
{
  public class MassTransitOptions
  {
    public enum TransportType
    {
      Undefined,
      RabbitMQ
    }
    public TransportType Type { get; set; } = TransportType.Undefined;

    public MassTransitRabbitMqOptions? RabbitMqOptions { get; set; }
  }

}
