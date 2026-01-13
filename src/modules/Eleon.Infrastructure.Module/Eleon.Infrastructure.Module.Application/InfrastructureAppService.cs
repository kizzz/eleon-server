using Infrastructure.Module.Localization;
using Volo.Abp.Application.Services;

namespace VPortal.Infrastructure.Module;

public abstract class InfrastructureAppService : ApplicationService
{
  protected InfrastructureAppService()
  {
    LocalizationResource = typeof(InfrastructureResource);
    ObjectMapperContext = typeof(ModuleApplicationModule);
  }
}
