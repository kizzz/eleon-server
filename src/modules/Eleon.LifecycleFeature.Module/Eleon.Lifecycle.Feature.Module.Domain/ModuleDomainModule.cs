using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.Lifecycle.Feature.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ModuleDomainSharedModule))]
public class ModuleDomainModule : AbpModule
{

}
