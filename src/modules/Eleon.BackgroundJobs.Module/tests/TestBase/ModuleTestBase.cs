using Eleon.TestsBase.Lib.TestBase;
using Volo.Abp;
using Volo.Abp.Modularity;
using VPortal.BackgroundJobs.Module;
using VPortal.BackgroundJobs.Module.EntityFrameworkCore;

namespace BackgroundJobs.Module.TestBase;

/* All test classes are derived from this class, directly or indirectly. */
public abstract class ModuleTestBase<TStartupModule> : AbpIntegratedTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
