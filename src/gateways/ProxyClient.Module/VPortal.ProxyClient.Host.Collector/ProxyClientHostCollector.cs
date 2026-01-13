using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using VPortal.Auditor.Module;
using VPortal.Auditor.Module.RemoteStore;
using VPortal.Business.Implementation.Module;
using VPortal.Identity.Module;
using VPortal.Infrastructure.Module;
using VPortal.ProxyClient.Collector;
using VPortal.Storage.Remote.HttpApi;

namespace VPortal.ProxyClient.Host.Collector
{
    [DependsOn(
        typeof(ProxyClientCollector),
        typeof(BusinessRemoteHttpApiModule),
        typeof(StorageRemoteHttpApiModule),
        typeof(IdentityHttpApiClientModule),
        typeof(AuditorHttpApiClientModule),
        typeof(InfrastructureUtilsHttpApiModule)
        )]
    public class ProxyClientHostCollector : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddRemoteAbpAuditingStore();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAuditingOptions>(options =>
            {
                options.IsEnabled = true;
                options.IsEnabledForGetRequests = true;
            });
        }
    }
}
