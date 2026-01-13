using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace VPortal.TenantManagement.Module;

[DependsOn(
    typeof(TenantManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule),
    typeof(AbpTenantManagementApplicationContractsModule)
    )]
public class TenantManagementApplicationContractsModule : AbpModule
{ }
