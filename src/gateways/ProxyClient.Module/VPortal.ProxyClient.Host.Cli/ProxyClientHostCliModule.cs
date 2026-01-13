using Volo.Abp.Modularity;
using VPortal.Identity.Module;
using VPortal.ProxyClient.Host.Collector;

namespace VPortal;

[DependsOn(
    typeof(ProxyClientHostCollector),
    typeof(IdentityHttpApiClientModule)
    )]
public class ProxyClientHostCliModule : AbpModule
{}
