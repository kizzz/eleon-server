using Volo.Abp.Modularity;
using VPortal.ApplicationConfiguration.Module.EntityFrameworkCore;

namespace VPortal.ApplicationConfiguration.Module
{
  [DependsOn(
      typeof(ApplicationConfigurationApplicationModule),
      typeof(ApplicationConfigurationHttpApiModule),
      typeof(ApplicationConfigurationEntityFrameworkCoreModule))]
  public class ApplicationConfigurationModuleCollector : AbpModule
  {
  }
}
