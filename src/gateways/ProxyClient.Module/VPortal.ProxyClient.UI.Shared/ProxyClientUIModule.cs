using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal.ProxyClient.Domain.Shared;
using VPortal.ProxyClient.UI.Windows;

namespace VPortal.ProxyClient.UI.Shared
{
    [DependsOn(
        typeof(ProxyClientDomainSharedModule),
        typeof(ProxyClientHostCliClientModule))]
    public class ProxyClientUIModule : AbpModule
    {
    }
}
