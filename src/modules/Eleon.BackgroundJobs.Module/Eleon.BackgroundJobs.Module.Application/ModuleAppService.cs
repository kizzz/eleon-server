using Volo.Abp.Application.Services;
using VPortal.BackgroundJobs.Module.Localization;

namespace VPortal.BackgroundJobs.Module;

public abstract class ModuleAppService : ApplicationService
{
  protected ModuleAppService()
  {
    LocalizationResource = typeof(ModuleResource);
    ObjectMapperContext = typeof(ModuleApplicationModule);
  }
}
