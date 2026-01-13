using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.LanguageManagement.Module;

[DependsOn(
    typeof(LanguageManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class LanguageManagementApplicationContractsModule : AbpModule
{ }
