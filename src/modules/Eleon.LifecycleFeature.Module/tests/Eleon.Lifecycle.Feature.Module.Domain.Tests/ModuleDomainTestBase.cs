using Volo.Abp.Modularity;

namespace VPortal.Lifecycle.Feature.Module;

/* Inherit from this class for your domain layer tests.
 * See LifecycleManagerDomainServiceTests for example.
 */
public abstract class ModuleDomainTestBase<TStartupModule> : ModuleTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

