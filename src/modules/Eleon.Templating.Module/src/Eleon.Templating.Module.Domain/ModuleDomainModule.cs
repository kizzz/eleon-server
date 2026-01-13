using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Eleon.Templating.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ModuleDomainSharedModule)
)]
public class ModuleDomainModule : AbpModule
{

}
