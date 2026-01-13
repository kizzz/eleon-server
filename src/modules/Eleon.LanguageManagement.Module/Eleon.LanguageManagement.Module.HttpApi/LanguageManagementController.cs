using Volo.Abp.AspNetCore.Mvc;
using VPortal.LanguageManagement.Module.Localization;

namespace VPortal.LanguageManagement.Module;

public abstract class LanguageManagementController : AbpControllerBase
{
  protected LanguageManagementController()
  {
    LocalizationResource = typeof(LanguageManagementResource);
  }
}
