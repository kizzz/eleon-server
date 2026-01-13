using Volo.Abp.Modularity;

namespace Eleon.Templating.Module;

[DependsOn(
    typeof(ModuleDomainModule),
    typeof(ModuleTestBaseModule)
)]
public class ModuleDomainTestModule : AbpModule
{

}
