using Volo.Abp.Modularity;

namespace VPortal.Infrastructure.Module
{
  [DependsOn(
  typeof(VPortal.Infrastructure.Module.ModuleHttpApiModule),
  typeof(VPortal.Infrastructure.Module.ModuleApplicationModule),
  typeof(VPortal.Infrastructure.Module.EntityFrameworkCore.InfrastructureEntityFrameworkCoreModule))]
  public class InfrastructureModuleCollector : AbpModule
  {

  }
}
