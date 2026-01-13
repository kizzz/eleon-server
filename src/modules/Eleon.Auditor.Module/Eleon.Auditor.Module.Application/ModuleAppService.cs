using Volo.Abp.Application.Services;
using VPortal.Auditor.Module.Localization;

namespace VPortal.Auditor.Module;

public abstract class ModuleAppService : ApplicationService
{
  protected ModuleAppService()
  {
    LocalizationResource = typeof(ModuleResource);
    ObjectMapperContext = typeof(ModuleApplicationModule);
  }
}
