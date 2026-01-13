using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.JobScheduler.Module;

[DependsOn(
    typeof(JobSchedulerDomainModule),
    typeof(JobSchedulerApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class JobSchedulerApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<JobSchedulerApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<JobSchedulerApplicationModule>(validate: true);
    });
  }
}
