using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal.GatewayClient.Domain.Shared;

namespace VPortal.GatewayClient.Domain.Windows
{
    [DependsOn(typeof(GatewayClientDomainSharedModule))]
    public class GatewayClientDomainWindowsModule : AbpModule
    {
    }
}
