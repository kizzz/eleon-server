using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.Collaboration.Feature.Module;

[DependsOn(
    typeof(CollaborationDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class CollaborationApplicationContractsModule : AbpModule
{ }
