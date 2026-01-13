using GatewayManagement.Module.Proxies;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.RemoteServices;
using VPortal.Identity.Module;
// using VPortal.Identity.Module.MachineKeyValidation;
using VPortal.GatewayManagement.Module;
using Common.Module;

namespace VPortal.GatewayManagement;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpRemoteServicesModule),
    typeof(AbpCachingModule),
    typeof(GatewayManagementDomainSharedModule)
// typeof(IdentityHttpApiClientModule)
)]
public class GatewayManagementDomainModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    PreConfigure<AbpHttpClientBuilderOptions>(options =>
    {
      // options.AddAppendMachineKeyHandler();
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    //context.Services.AddGatewayDynamicConfiguration();
    //context.Services.AddGatewayHostMachineSecretsProvider();
  }
}
