using Microsoft.Extensions.DependencyInjection;
using ModuleCollector.UnifiedMigrations;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VPortal.Unified.Module.EntityFrameworkCore;

[DependsOn(
    typeof(AbpEntityFrameworkCoreModule)
)]
public class UnifiedEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<UnifiedDbContext>(options =>
        {
        });
    }
}
