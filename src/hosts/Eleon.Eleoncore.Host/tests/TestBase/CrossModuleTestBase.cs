using Eleon.TestsBase.Lib.TestBase;
using Volo.Abp;
using Volo.Abp.Modularity;
using Eleonsoft.Host;

namespace Eleonsoft.Host.Test.TestBase;

/// <summary>
/// Base class for cross-module integration tests
/// Uses CrossModuleTestStartupModule which loads MaximalHostModule with in-memory event bus
/// </summary>
public abstract class CrossModuleTestBase : AbpIntegratedTestBase<CrossModuleTestStartupModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
