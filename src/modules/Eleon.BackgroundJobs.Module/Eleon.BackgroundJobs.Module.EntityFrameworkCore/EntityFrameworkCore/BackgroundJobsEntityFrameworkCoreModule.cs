using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.Repositories;

namespace VPortal.BackgroundJobs.Module.EntityFrameworkCore;

[DependsOn(
    typeof(ModuleDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class BackgroundJobsEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<BackgroundJobsDbContext>(options =>
    {
      options.AddRepository<BackgroundJobExecutionEntity, BackgroundJobExecutionsRepository>();
      options.AddRepository<BackgroundJobEntity, BackgroundJobsRepository>();
      options.AddRepository<BackgroundJobMessageEntity, BackgroundJobMessagesRepository>();
      /* Add custom repositories here. Example:
       * options.AddRepository<Question, EfCoreQuestionRepository>();
       */
    });
  }
}
