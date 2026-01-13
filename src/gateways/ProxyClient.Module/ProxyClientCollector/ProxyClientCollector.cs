using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using VPortal.Core.Infrastructure.Module;
using VPortal.DataSource.Module;
using VPortal.Identity.Module;
using VPortal.ProxyClient.Domain;
using VPortal.Storage.Module;

namespace VPortal.ProxyClient.Collector
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(CoreInfrastructureHttpApiClientModule),
        typeof(StorageHttpApiClientModule),
        typeof(ProxyClientDomainModule)
        )]
    public class ProxyClientCollector : AbpModule
    {
    }
}
