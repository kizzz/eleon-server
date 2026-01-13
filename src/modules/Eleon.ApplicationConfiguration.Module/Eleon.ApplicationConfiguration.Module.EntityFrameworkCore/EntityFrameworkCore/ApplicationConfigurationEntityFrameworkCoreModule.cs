using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using VPortal.Otp;

namespace VPortal.ApplicationConfiguration.Module.EntityFrameworkCore;

[DependsOn(
    typeof(ApplicationConfigurationDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class ApplicationConfigurationEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<ApplicationConfigurationDbContext>(options =>
    {
      /* Add custom repositories here. Example:
       * options.AddRepository<Question, EfCoreQuestionRepository>();
       */
    });
  }
}
