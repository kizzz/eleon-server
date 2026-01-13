using Volo.Abp.AspNetCore.Mvc;
using VPortal.Accounting.Module.Localization;

namespace VPortal.Accounting.Module;

public abstract class ModuleController : AbpControllerBase
{
  protected ModuleController()
  {
    LocalizationResource = typeof(AccountingResource);
  }
}
