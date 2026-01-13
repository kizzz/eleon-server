using Volo.Abp.Modularity;
using VPortal.BackgroundJobs.Module.EntityFrameworkCore;

namespace VPortal.BackgroundJobs.Module
{
  [DependsOn(
      typeof(ModuleHttpApiModule),
      typeof(ModuleApplicationModule),
      typeof(BackgroundJobsEntityFrameworkCoreModule))]
  public class BackgroundJobsModuleCollector : AbpModule
  {

  }
}
