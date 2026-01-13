using Volo.Abp.Modularity;

namespace VPortal.Auditor.Module
{
  [DependsOn(
  typeof(VPortal.Auditor.Module.ModuleHttpApiModule),
  typeof(VPortal.Auditor.Module.ModuleApplicationModule),
  typeof(VPortal.Auditor.Module.EntityFrameworkCore.AuditorEntityFrameworkCoreModule))]
  public class AuditorModuleCollector : AbpModule
  {

  }
}
