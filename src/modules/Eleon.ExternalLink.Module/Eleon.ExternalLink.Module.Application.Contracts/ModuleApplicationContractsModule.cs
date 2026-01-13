using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.ExternalLink.Module;

[DependsOn(
    typeof(ModuleDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class ModuleApplicationContractsModule : AbpModule
{ }
