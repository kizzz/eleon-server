using Volo.Abp.Application.Services;
using VPortal.Localization;

namespace VPortal;

/* Inherit your application services from this class.
 */
public abstract class VPortalAppService : ApplicationService
{
  protected VPortalAppService()
  {
    LocalizationResource = typeof(VPortalResource);
  }
}
