using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal.Infrastructure.Module;
using VPortal.ProxyClient.Domain.Shared;

namespace VPortal.ProxyClient.UI.Windows
{
    [DependsOn(
        typeof(MinimalInfrastructureDomainModule),
        typeof(ProxyClientDomainSharedModule))]
    public class ProxyClientHostCliClientModule : AbpModule
    {
    }
}
