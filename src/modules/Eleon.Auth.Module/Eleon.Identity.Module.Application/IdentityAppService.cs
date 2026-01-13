using Volo.Abp.Application.Services;
using VPortal.Identity.Module.Localization;

namespace VPortal.Identity.Module;

public abstract class IdentityAppService : ApplicationService
{
  protected IdentityAppService()
  {
    LocalizationResource = typeof(IdentityResource);
    ObjectMapperContext = typeof(IdentityApplicationModule);
  }
}
