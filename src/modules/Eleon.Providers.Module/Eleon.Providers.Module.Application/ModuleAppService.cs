using Volo.Abp.Application.Services;
using VPortal.Storage.Module.Localization;

namespace VPortal.Storage.Module;

public abstract class ModuleAppService : ApplicationService
{
  protected ModuleAppService()
  {
    LocalizationResource = typeof(StorageModuleResource);
    ObjectMapperContext = typeof(ProviderApplicationModule);
  }
}
