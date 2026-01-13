using Volo.Abp.Modularity;
using VPortal.LanguageManagement.Module.EntityFrameworkCore;

namespace VPortal.LanguageManagement.Module
{
  [DependsOn(
      typeof(LanguageManagementApplicationModule),
      typeof(LanguageManagementHttpApiModule),
      typeof(LanguageManagementEntityFrameworkCoreModule))]
  public class LanguageManagementModuleCollector : AbpModule
  {
  }
}
