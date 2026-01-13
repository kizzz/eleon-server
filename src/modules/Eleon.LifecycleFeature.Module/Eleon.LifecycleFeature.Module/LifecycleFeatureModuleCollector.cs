using Volo.Abp.Modularity;

namespace VPortal.Lifecycle.Feature.Module
{
  [DependsOn(
  typeof(VPortal.Lifecycle.Feature.Module.ModuleHttpApiModule),
  typeof(VPortal.Lifecycle.Feature.Module.ModuleApplicationModule),
  typeof(VPortal.Lifecycle.Feature.Module.EntityFrameworkCore.LifecycleFeatureEntityFrameworkCoreModule))]
  public class LifecycleFeatureModuleCollector : AbpModule
  {

  }
}
