using Volo.Abp.Auditing;
using Volo.Abp.Modularity;
using VPortal.TenantManagement.Module.EntityFrameworkCore;

namespace VPortal.TenantManagement.Module
{
  [DependsOn(
      typeof(TenantManagementApplicationModule),
      typeof(TenantManagementHttpApiModule),
      typeof(TenantManagementEntityFrameworkCoreModule))]
  public class TenantManagementModuleCollector : AbpModule
  {
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
      Configure<AbpAuditingOptions>(options =>
      {
        options.EntityHistorySelectors.AddAllEntities();
      });
    }
  }
}
