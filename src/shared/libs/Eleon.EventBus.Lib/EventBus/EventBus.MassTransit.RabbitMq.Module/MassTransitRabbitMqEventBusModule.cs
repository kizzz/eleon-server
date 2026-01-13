using Common.EventBus.Abstractions.Module;
using Volo.Abp.Modularity;

namespace EventBus.MassTransit.Module
{
  [DependsOn(typeof(CommonEventBusAbstractionsModule))]
  public class MassTransitRabbitMqEventBusModule : AbpModule
  {
  }
}
