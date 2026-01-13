using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.EventManagementModule.Module;

[DependsOn(
    typeof(EventManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class EventManagementApplicationContractsModule : AbpModule
{ }
