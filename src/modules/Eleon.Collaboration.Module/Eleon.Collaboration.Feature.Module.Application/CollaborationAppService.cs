using Volo.Abp.Application.Services;
using VPortal.Collaboration.Feature.Module.Localization;

namespace VPortal.Collaboration.Feature.Module;

public abstract class CollaborationAppService : ApplicationService
{
  protected CollaborationAppService()
  {
    LocalizationResource = typeof(CollaborationResource);
    ObjectMapperContext = typeof(CollaborationApplicationModule);
  }
}
