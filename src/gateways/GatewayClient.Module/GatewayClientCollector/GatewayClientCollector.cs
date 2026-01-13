using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using VPortal.GatewayClient.Domain;

namespace VPortal.GatewayClient.Collector
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(GatewayClientDomainModule)
        )]
    public class GatewayClientCollector : AbpModule
    {
    }
}
