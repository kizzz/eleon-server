using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal.GatewayClient.Domain.Shared;
using Common.Module;

namespace VPortal.GatewayClient.UI.Windows
{
    [DependsOn(
        typeof(CommonModule),
        typeof(GatewayClientDomainSharedModule))]
    public class GatewayClientHostCliClientModule : AbpModule
    {
    }
}
