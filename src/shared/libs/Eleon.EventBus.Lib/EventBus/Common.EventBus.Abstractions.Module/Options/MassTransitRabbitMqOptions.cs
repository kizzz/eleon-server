namespace VPortal.MassTransit.RabbitMQ
{
  public class MassTransitRabbitMqOptions
  {
    public required string Host { get; set; }
    public ushort Port { get; set; }
    public required string VirtualHost { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool UseSsl { get; set; }
    public bool UseCluster { get; set; }
    public required List<string> ClusterNodes { get; set; }
    public required string DefaultQueuePrefix { get; set; }
    public int DefaultConcurrentMessageLimit { get; set; } = 1;
    public int DefaultPrefetchCount { get; set; } = 4;
    public bool DefaultDurable { get; set; } = true;
    public bool DefaultAutoDelete { get; set; } = false;
    public string DefaultExchangeType { get; set; } = "fanout";
  }
}
