using Eleon.TestsBase.Lib.TestBase;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace VPortal.FileManager.Module.Tests.TestBase;

/// <summary>
/// Base class for integration tests in FileManager module.
/// </summary>
public abstract class ModuleTestBase<TStartupModule> : AbpIntegratedTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
