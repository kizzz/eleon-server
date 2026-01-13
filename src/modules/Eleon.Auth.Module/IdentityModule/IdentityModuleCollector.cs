using Volo.Abp.Data;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using VPortal.Identity.Module.EntityFrameworkCore;

namespace VPortal.Identity.Module
{
  [DependsOn(
      typeof(IdentityApplicationModule),
      typeof(IdentityHttpApiModule),
      typeof(IdentityEntityFrameworkCoreModule))]
  public class IdentityModuleCollector : AbpModule
  {
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
      AbpCommonDbProperties.DbTablePrefix = "Ec";
      AbpIdentityServerDbProperties.DbTablePrefix = "EcIdentityServer";
    }
  }
}
