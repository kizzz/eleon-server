using Eleon.InternalProxy.App;
using Eleoncore;
using Volo.Abp.Modularity;
using VPortal;

namespace Eleonsoft.ProxyAgent;

[DependsOn(
   typeof(EleonHostModule),
    typeof(InternalProxyModule)
//typeof(OrchestratorApplication)

)]
public class EleonProxyHostModule : AbpModule
{

}
