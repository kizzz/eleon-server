using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using VPortal.Core.Infrastructure.Module.Entities;
using VPortal.Core.Infrastructure.Module.Repositories;

namespace VPortal.Infrastructure.Module.EntityFrameworkCore;

[DependsOn(
    typeof(InfrastructureDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class InfrastructureEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<InfrastructureDbContext>(options =>
    {
      options.AddRepository<FeatureSettingEntity, FeatureSettingsRepository>();
    });
  }
}
