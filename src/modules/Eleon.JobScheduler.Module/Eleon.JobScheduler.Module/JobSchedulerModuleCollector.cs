using Volo.Abp.Modularity;
using VPortal.JobScheduler.Module.EntityFrameworkCore;

namespace VPortal.JobScheduler.Module
{
  [DependsOn(
      typeof(JobSchedulerApplicationModule),
      typeof(ModuleHttpApiModule),
      typeof(JobSchedulerEntityFrameworkCoreModule))]
  public class JobSchedulerModuleCollector : AbpModule
  {
  }
}
