using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using VPortal.ApplicationConfiguration.Module;

namespace VPortal.Otp;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(ApplicationConfigurationDomainSharedModule)
)]
public class ApplicationConfigurationDomainModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
  }
}

