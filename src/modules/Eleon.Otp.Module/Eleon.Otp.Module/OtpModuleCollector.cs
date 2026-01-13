using Volo.Abp.Modularity;
using VPortal.Otp.Module.EntityFrameworkCore;

namespace VPortal.Otp.Module
{
  [DependsOn(
      typeof(OtpApplicationModule),
      typeof(OtpHttpApiModule),
      typeof(OtpEntityFrameworkCoreModule))]
  public class OtpModuleCollector : AbpModule
  {
  }
}
