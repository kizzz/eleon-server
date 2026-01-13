using Volo.Abp.Application.Services;
using VPortal.Accounting.Module.Localization;

namespace VPortal.Accounting.Module;

public abstract class ModuleAppService : ApplicationService
{
  protected ModuleAppService()
  {
    LocalizationResource = typeof(AccountingResource);
    ObjectMapperContext = typeof(AccountingApplicationModule);
  }
}
