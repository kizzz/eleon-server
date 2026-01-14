using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.SystemServicesModule.Module.EntityFrameworkCore;

[DependsOn(
    typeof(SystemServicesModuleDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class SystemServicesModuleEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<SystemServicesModuleDbContext>(options =>
    {
      options.AddDefaultRepositories();
    });
  }
}

