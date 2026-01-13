using Eleon.Templating.Module.Localization;
using Volo.Abp.Application.Services;

namespace Eleon.Templating.Module;

public abstract class ModuleAppService : ApplicationService
{
  protected ModuleAppService()
  {
    LocalizationResource = typeof(TemplatingResource);
    ObjectMapperContext = typeof(TemplatingApplicationModule);
  }
}
