using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace VPortal.JobScheduler.Module;

[DependsOn(
    typeof(JobSchedulerDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
    )]
public class JobSchedulerApplicationContractsModule : AbpModule
{ }
