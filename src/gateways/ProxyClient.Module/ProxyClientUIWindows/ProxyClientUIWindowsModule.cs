using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal.ProxyClient.Domain.Windows;
using VPortal.ProxyClient.UI.Shared;

namespace VPortal.ProxyClient.UI.Windows
{
    [DependsOn(
        typeof(ProxyClientUIModule),
        typeof(ProxyClientDomainWindowsModule))]
    public class ProxyClientUIWindowsModule : AbpModule
    {
    }
}
