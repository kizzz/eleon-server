using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.JobScheduler.Module.EntityFrameworkCore;

[DependsOn(
    typeof(JobSchedulerDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class JobSchedulerEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<JobSchedulerDbContext>(options =>
    {
      /* Add custom repositories here. Example:
       * options.AddRepository<Question, EfCoreQuestionRepository>();
       */
    });
  }
}
