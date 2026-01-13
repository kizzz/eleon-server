using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal.GatewayClient.Domain.Shared;
using VPortal.GatewayClient.UI.Windows;

namespace VPortal.GatewayClient.UI.Shared
{
    [DependsOn(
        typeof(GatewayClientDomainSharedModule),
        typeof(GatewayClientHostCliClientModule))]
    public class GatewayClientUIModule : AbpModule
    {
    }
}
