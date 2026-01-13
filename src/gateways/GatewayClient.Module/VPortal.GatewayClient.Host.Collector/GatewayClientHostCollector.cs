using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using VPortal.Identity.Module;
using VPortal.Infrastructure.Module;
using VPortal.GatewayClient.Collector;

namespace VPortal.GatewayClient.Host.Collector
{
    [DependsOn(
        typeof(GatewayClientCollector),
        typeof(IdentityHttpApiClientModule)
        )]
    public class GatewayClientHostCollector : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            //context.Services.AddRemoteAbpAuditingStore();
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
