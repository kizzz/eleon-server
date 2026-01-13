using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.DocMessageLog.Module;

[DependsOn(
    typeof(SystemLogDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class ModuleApplicationContractsModule : AbpModule
{ }
