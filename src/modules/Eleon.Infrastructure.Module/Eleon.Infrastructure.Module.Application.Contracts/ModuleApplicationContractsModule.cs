using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.Infrastructure.Module;

[DependsOn(
    typeof(InfrastructureDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule))]
public class ModuleApplicationContractsModule : AbpModule
{ }
