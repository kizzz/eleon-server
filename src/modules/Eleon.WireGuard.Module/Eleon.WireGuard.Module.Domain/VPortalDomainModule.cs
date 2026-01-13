using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace VPortal;

[DependsOn(
    typeof(VPortalDomainSharedModule),
    typeof(AbpCachingModule)
    )]
public class VPortalDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {

  }
}
