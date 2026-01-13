using Volo.Abp.Application.Services;
using VPortal.ExternalLink.Module.Localization;

namespace VPortal.ExternalLink.Module;

public abstract class ModuleAppService : ApplicationService
{
  protected ModuleAppService()
  {
    LocalizationResource = typeof(ModuleResource);
    ObjectMapperContext = typeof(ModuleApplicationModule);
  }
}
