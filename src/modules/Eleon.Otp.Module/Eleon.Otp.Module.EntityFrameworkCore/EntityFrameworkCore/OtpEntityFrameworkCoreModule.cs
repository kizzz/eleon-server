using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.Otp.Module.EntityFrameworkCore;

[DependsOn(
    typeof(OtpDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class OtpEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<OtpDbContext>(options =>
    {
      /* Add custom repositories here. Example:
       * options.AddRepository<Question, EfCoreQuestionRepository>();
       */
    });
  }
}
