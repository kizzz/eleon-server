using Volo.Abp.AspNetCore.Mvc;
using VPortal.Identity.Module.Localization;

namespace VPortal.Identity.Module;

public abstract class IdentityController : AbpControllerBase
{
  protected IdentityController()
  {
    LocalizationResource = typeof(IdentityResource);
  }
}
