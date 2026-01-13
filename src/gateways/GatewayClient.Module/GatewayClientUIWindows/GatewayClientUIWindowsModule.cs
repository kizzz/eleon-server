using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal.GatewayClient.Domain.Windows;
using VPortal.GatewayClient.UI.Shared;

namespace VPortal.GatewayClient.UI.Windows
{
    [DependsOn(
        typeof(GatewayClientUIModule),
        typeof(GatewayClientDomainWindowsModule))]
    public class GatewayClientUIWindowsModule : AbpModule
    {
    }
}
