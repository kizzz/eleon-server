using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.SystemServicesModule.Module;

[DependsOn(
    typeof(SystemServicesDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class SystemServicesApplicationContractsModule : AbpModule
{ }

