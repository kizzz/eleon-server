using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.Auditor.Module.EntityFrameworkCore;

[DependsOn(
    typeof(ModuleDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class AuditorEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<AuditorDbContext>(options =>
    {
      /* Add custom repositories here. Example:
       * options.AddRepository<Question, EfCoreQuestionRepository>();
       */
    });
  }
}
