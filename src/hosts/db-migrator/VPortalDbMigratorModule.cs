using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;
using VPortal.EntityFrameworkCore;
using VPortal.Unified.Module.EntityFrameworkCore;

namespace VPortal.DbMigrator;

[DependsOn(
    // Runtime dependencies for migrator logic
    typeof(AbpAutofacModule),
    typeof(VPortalApplicationContractsModule),

    // ABP EF modules are refenced through VPortalEntityFrameworkCoreModule
    typeof(VPortalEntityFrameworkCoreModule),

    // Essential VPortal modules
    typeof(UnifiedEntityFrameworkCoreModule)
)]
public class VPortalDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }
}
