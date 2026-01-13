using Common.EventBus.Abstractions.Module;
using Volo.Abp.Modularity;

namespace EventBus.LocalBus.Module
{
  [DependsOn(typeof(CommonEventBusAbstractionsModule))]
  public class EventBusLocalBusModule : AbpModule
  {
  }
}
