using Volo.Abp.Application.Services;
using VPortal.Lifecycle.Feature.Module.Localization;

namespace VPortal.Lifecycle.Feature.Module;

public abstract class ModuleAppService : ApplicationService
{
  protected ModuleAppService()
  {
    LocalizationResource = typeof(LifecycleFeatureModuleResource);
    ObjectMapperContext = typeof(ModuleApplicationModule);
  }
}
