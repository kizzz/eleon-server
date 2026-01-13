using Common.EventBus.Abstractions.Module;
using Volo.Abp.Modularity;

namespace EventBus.Nats
{
  [DependsOn(typeof(CommonEventBusAbstractionsModule))]
  public class NatsEventBusModule : AbpModule
  {
  }
}
