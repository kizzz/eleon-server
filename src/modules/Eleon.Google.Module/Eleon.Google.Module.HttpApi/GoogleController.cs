using Volo.Abp.AspNetCore.Mvc;
using VPortal.Google.Module.Localization;

namespace VPortal.Google.Module;

public abstract class GoogleController : AbpControllerBase
{
  protected GoogleController()
  {
    LocalizationResource = typeof(GoogleResource);
  }
}
