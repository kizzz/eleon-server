using Volo.Abp.Data;
using Volo.Abp.Modularity;
using VPortal.SitesManagement.Module.EntityFrameworkCore;

namespace VPortal.SitesManagement.Module
{
  [DependsOn(
      typeof(SitesManagementApplicationModule),
      typeof(SitesManagementHttpApiModule),
      typeof(SitesManagementEntityFrameworkCoreModule))]
  public class SitesManagementModuleCollector : AbpModule
  {
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
      AbpCommonDbProperties.DbTablePrefix = "Ec";
    }
  }
}

