using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.JobScheduler.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(JobSchedulerDomainSharedModule))]
public class JobSchedulerDomainModule : AbpModule
{

}
