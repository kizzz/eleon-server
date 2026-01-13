using Volo.Abp.DependencyInjection;
using Volo.Abp.RabbitMQ;

namespace EventBus.RabbitMq.Module
{
  public class RabbitMqDynamicConnections : RabbitMqConnections, ISingletonDependency
  {
  }
}
