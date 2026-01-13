using Eleoncore;
using Eleoncore.Proxy.App;
using Volo.Abp.Modularity;
using VPortal;

namespace Eleonsoft.Host;

[DependsOn(
    typeof(EleoncoreHostModule),
    typeof(EleoncoreProxyModule)
    )]
public class EleoncoreProxyHostModule : AbpModule
{
}
