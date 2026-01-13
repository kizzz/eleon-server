using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.HealthCheckModule.Module.EntityFrameworkCore;

[DependsOn(
    typeof(HealthCheckModuleDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class HealthCheckModuleEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<HealthCheckModuleDbContext>(options =>
    {
      options.AddDefaultRepositories();
    });
  }
}
