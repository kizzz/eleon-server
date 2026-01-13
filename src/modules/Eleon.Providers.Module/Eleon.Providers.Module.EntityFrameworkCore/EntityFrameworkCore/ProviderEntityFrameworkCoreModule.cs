using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.Storage.Module.EntityFrameworkCore;

[DependsOn(
    typeof(ProviderDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class ProviderEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<StorageDbContext>(options =>
    {
      /* Add custom repositories here. Example:
       * options.AddRepository<Question, EfCoreQuestionRepository>();
       */
    });
  }
}
