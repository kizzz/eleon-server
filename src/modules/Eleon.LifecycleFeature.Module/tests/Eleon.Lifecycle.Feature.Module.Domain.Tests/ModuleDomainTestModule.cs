using Eleon.Logging.Lib.VportalLogging;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace VPortal.Lifecycle.Feature.Module;

[DependsOn(
    typeof(ModuleDomainModule),
    typeof(ModuleTestBaseModule)
)]
public class ModuleDomainTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
        context.Services.AddLogging();
        context.Services.AddVportalLogging();
    }
}
