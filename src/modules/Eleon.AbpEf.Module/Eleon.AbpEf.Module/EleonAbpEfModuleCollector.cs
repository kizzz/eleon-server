using EleoncoreMultiTenancy.Module.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using VPortal.EleoncoreMultiTenancy.Module;

namespace EleoncoreMultiTenancy.Module
{
  [DependsOn(
      typeof(EleonAbpEfApplicationModule),
      typeof(EleonAbpEfHttpApiModule),
      typeof(EleonAbpEfEntityFrameworkCoreModule))]
  public class EleonAbpEfModuleCollector : AbpModule
  {
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
      AbpCommonDbProperties.DbTablePrefix = "Ec";
      AbpBackgroundJobsDbProperties.DbTablePrefix = "EcAbp";
    }
  }
}
