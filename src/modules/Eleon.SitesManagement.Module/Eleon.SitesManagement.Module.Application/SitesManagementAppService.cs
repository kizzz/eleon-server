using Volo.Abp.Application.Services;
using VPortal.SitesManagement.Module.Localization;

namespace VPortal.SitesManagement.Module;

public abstract class SitesManagementAppService : ApplicationService
{
  protected SitesManagementAppService()
  {
    LocalizationResource = typeof(SitesManagementResource);
    ObjectMapperContext = typeof(SitesManagementApplicationModule);
  }
}

