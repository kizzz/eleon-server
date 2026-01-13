using Volo.Abp.Application.Services;
using VPortal.LanguageManagement.Module.Localization;

namespace VPortal.LanguageManagement.Module;

public abstract class LanguageManagementAppService : ApplicationService
{
  protected LanguageManagementAppService()
  {
    LocalizationResource = typeof(LanguageManagementResource);
    ObjectMapperContext = typeof(LanguageManagementApplicationModule);
  }
}
