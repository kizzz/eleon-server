using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Repositories.Audits;
using VPortal.Lifecycle.Feature.Module.Repositories.Templates;

namespace VPortal.Lifecycle.Feature.Module.EntityFrameworkCore;

[DependsOn(
    typeof(ModuleDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class LifecycleFeatureEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<LifecycleFeatureDbContext>(options =>
    {
      options.AddRepository<StatesGroupAuditEntity, StatesGroupAuditsRepository>();
      options.AddRepository<StatesGroupTemplateEntity, StatesGroupTemplatesRepository>();
    });
  }
}
