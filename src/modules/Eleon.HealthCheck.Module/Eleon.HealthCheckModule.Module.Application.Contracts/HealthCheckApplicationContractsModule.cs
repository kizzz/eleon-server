using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.HealthCheckModule.Module;

[DependsOn(
    typeof(HealthCheckDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class HealthCheckApplicationContractsModule : AbpModule
{ }
