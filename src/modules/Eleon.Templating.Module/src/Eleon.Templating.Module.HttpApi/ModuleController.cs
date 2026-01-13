using Eleon.Templating.Module.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Eleon.Templating.Module;

public abstract class ModuleController : AbpControllerBase
{
  protected ModuleController()
  {
    LocalizationResource = typeof(TemplatingResource);
  }
}
