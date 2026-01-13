using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.ExternalLink.Module.EntityFrameworkCore;

[DependsOn(
    typeof(ExternalLinkDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class ExternalLinkEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<ExternalLinkDbContext>(options =>
    {
      /* Add custom repositories here. Example:
       * options.AddRepository<Question, EfCoreQuestionRepository>();
       */
      options.AddDefaultRepositories();
    });
  }
}
