using Volo.Abp.Application.Services;
using VPortal.FileManager.Module.Localization;

namespace VPortal.FileManager.Module;

public abstract class ModuleAppService : ApplicationService
{
  protected ModuleAppService()
  {
    LocalizationResource = typeof(ModuleResource);
    ObjectMapperContext = typeof(ModuleApplicationModule);
  }
}
