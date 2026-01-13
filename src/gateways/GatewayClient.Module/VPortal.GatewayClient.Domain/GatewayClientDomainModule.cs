using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using VPortal.GatewayManagement.Module;
using VPortal.Identity.Module;
using VPortal.Identity.Module.MachineKeyValidation;
using VPortal.GatewayClient.Domain.Shared;
using Common.Module;

namespace VPortal.GatewayClient.Domain
{
    [DependsOn(
        typeof(GatewayClientDomainSharedModule),
        typeof(IdentityHttpApiClientModule),
        typeof(CommonModule),
        typeof(GatewayManagementHttpApiClientModule))]
    public class GatewayClientDomainModule: AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpHttpClientBuilderOptions>(options =>
            {
                options.AddAppendMachineKeyHandler();
            });
        }
    }
}
