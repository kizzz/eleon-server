using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using VPortal.SitesManagement.Module;

namespace VPortal.SitesManagement.Module;

[DependsOn(
    typeof(SitesManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class SitesManagementApplicationContractsModule : AbpModule
{ }


