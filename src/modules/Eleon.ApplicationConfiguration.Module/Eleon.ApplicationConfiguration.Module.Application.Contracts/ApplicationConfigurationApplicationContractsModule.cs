using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.ApplicationConfiguration.Module;

[DependsOn(
    typeof(ApplicationConfigurationDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class ApplicationConfigurationApplicationContractsModule : AbpModule
{ }
