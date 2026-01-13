using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.Collaboration.Feature.Module.EntityFrameworkCore;

[DependsOn(
    typeof(CollaborationDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class CollaborationEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<CollaborationDbContext>(options =>
    {
      /* Add custom repositories here. Example:
       * options.AddRepository<Question, EfCoreQuestionRepository>();
       */
    });
  }
}
