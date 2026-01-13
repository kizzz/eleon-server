using Volo.Abp.AspNetCore.Mvc;
using VPortal.Localization;

namespace VPortal.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class VPortalController : AbpControllerBase
{
  protected VPortalController()
  {
    LocalizationResource = typeof(VPortalResource);
  }
}
