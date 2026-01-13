using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.Google.Module;

[DependsOn(
    typeof(GoogleDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class GoogleApplicationContractsModule : AbpModule
{ }
