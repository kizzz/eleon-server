using Volo.Abp.Modularity;
using VPortal.GatewayManagement.Module.EntityFrameworkCore;

namespace VPortal.GatewayManagement.Module
{
  [DependsOn(
      typeof(GatewayManagementApplicationModule),
      typeof(GatewayManagementHttpApiModule),
      typeof(GatewayManagementEntityFrameworkCoreModule))]
  public class GatewayManagementModuleCollector : AbpModule
  {
  }
}
