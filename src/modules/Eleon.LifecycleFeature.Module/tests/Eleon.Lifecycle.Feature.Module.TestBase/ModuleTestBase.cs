using Eleon.TestsBase.Lib.TestBase;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace VPortal.Lifecycle.Feature.Module;

/* All test classes are derived from this class, directly or indirectly. */
public abstract class ModuleTestBase<TStartupModule> : AbpIntegratedTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
