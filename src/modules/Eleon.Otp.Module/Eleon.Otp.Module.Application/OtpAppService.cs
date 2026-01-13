using Volo.Abp.Application.Services;
using VPortal.Otp.Module.Localization;

namespace VPortal.Otp.Module;

public abstract class OtpAppService : ApplicationService
{
  protected OtpAppService()
  {
    LocalizationResource = typeof(OtpResource);
    ObjectMapperContext = typeof(OtpApplicationModule);
  }
}
