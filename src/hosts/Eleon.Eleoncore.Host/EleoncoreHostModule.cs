using Volo.Abp.Modularity;
using VPortal;
using VPortal.GatewayManagement.Module;
using VPortal.SitesManagement.Module;

namespace Eleonsoft.Host;

[DependsOn(
    typeof(EleonHostModule),
    typeof(SitesManagementModuleCollector),
    typeof(GatewayManagementModuleCollector)
    )]
public class EleoncoreHostModule : AbpModule
{
}
