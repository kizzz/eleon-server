using Common.Module;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace Common.EventBus.Abstractions.Module
{
  [DependsOn(typeof(AbpEventBusModule))]
  public class CommonEventBusAbstractionsModule : AbpModule
  {

  }
}
