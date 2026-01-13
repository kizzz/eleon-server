using Volo.Abp.Modularity;

namespace Eleon.Templating.Module;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class ModuleApplicationTestBase<TStartupModule> : ModuleTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
