using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using VPortal.ProxyManagement.Module;
using VPortal.Identity.Module;
using VPortal.Identity.Module.MachineKeyValidation;
using VPortal.Infrastructure.Module;
using VPortal.ProxyClient.Domain.Shared;
using VPortal.DataSource.Module;

namespace VPortal.ProxyClient.Domain
{
    [DependsOn(
        typeof(MinimalInfrastructureDomainModule),
        typeof(ProxyClientDomainSharedModule),
        typeof(IdentityHttpApiClientModule),
        typeof(DataSourceHttpApiClientModule),
        typeof(ProxyManagementHttpApiClientModule))]
    public class ProxyClientDomainModule: AbpModule
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
