using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using VPortal.Otp.Module;

namespace VPortal.Otp;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(OtpDomainSharedModule)
)]
public class OtpDomainModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
  }
}

