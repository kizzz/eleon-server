using EleoncoreMultiTenancy.Module.EntityFrameworkCore;
using Volo.Abp.Modularity;
using VPortal.EleoncoreMultiTenancy.Module;

namespace EleoncoreMultiTenancy.Module
{
  [DependsOn(
      typeof(EleoncoreMultiTenancyApplicationModule),
      typeof(EleoncoreMultiTenancyHttpApiModule),
      typeof(EleoncoreMultiTenancyEntityFrameworkCoreModule))]
  public class EleoncoreMultiTenancyModule : AbpModule
  {
  }
}
