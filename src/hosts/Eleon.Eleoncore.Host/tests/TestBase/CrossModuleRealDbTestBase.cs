using Eleon.TestsBase.Lib.TestBase;
using Volo.Abp;
using Volo.Abp.Modularity;
using Eleonsoft.Host;

namespace Eleonsoft.Host.Test.TestBase;

/// <summary>
/// Base class for cross-module integration tests that target a real database.
/// </summary>
public abstract class CrossModuleRealDbTestBase : AbpIntegratedTestBase<CrossModuleRealDbTestStartupModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
