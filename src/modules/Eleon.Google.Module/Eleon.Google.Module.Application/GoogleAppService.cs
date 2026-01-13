using Volo.Abp.Application.Services;
using VPortal.Google.Module.Localization;

namespace VPortal.Google.Module;

public abstract class GoogleAppService : ApplicationService
{
  protected GoogleAppService()
  {
    LocalizationResource = typeof(GoogleResource);
    ObjectMapperContext = typeof(GoogleApplicationModule);
  }
}
