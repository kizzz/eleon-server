namespace Common.Module.Constants
{
  public enum EventBusProvider
  {
    Undefined = 0,
    NATS = 1,
    RabbitMQ = 2,
    MassTransit = 3,
    InMemory = 4
  }
}
