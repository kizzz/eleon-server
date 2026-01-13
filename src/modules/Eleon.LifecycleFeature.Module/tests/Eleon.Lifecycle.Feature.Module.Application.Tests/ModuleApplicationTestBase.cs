using Volo.Abp.Modularity;

namespace VPortal.Lifecycle.Feature.Module;

/* Inherit from this class for your application layer tests.
 * See LifecycleManagerAppServiceTests for example.
 */
public abstract class ModuleApplicationTestBase<TStartupModule> : ModuleTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

