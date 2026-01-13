using Volo.Abp.Modularity;
using VPortal.Google.Module.EntityFrameworkCore;

namespace VPortal.Google.Module
{
  [DependsOn(
      typeof(GoogleApplicationModule),
      typeof(GoogleHttpApiModule),
      typeof(GoogleEntityFrameworkCoreModule))]
  public class GoogleModuleCollector : AbpModule
  {
  }
}
