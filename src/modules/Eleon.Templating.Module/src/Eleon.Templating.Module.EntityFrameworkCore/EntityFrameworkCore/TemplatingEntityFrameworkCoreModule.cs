using Eleon.Templating.Module.EntityFrameworkCore.Templates;
using Eleon.Templating.Module.Templates;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Eleon.Templating.Module.EntityFrameworkCore;

[DependsOn(
    typeof(ModuleDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class TemplatingEntityFrameworkCoreModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<TemplatingDbContext>(options =>
    {
      options.AddDefaultRepositories<ITemplatingDbContext>(includeAllEntities: true);

      options.AddRepository<Template, EfCoreTemplateRepository>();
    });
  }
}
