using Volo.Abp.Modularity;
using VPortal.Identity.Module;
using VPortal.GatewayClient.Host.Collector;
using Common.HttpApi.Module;

namespace VPortal;

[DependsOn(
    typeof(GatewayClientHostCollector),
    typeof(CommonHttpApiModule),
    typeof(IdentityHttpApiClientModule)
    )]
public class GatewayClientHostCliModule : AbpModule
{}
