using Volo.Abp.AspNetCore.Mvc;
using VPortal.Otp.Module.Localization;

namespace VPortal.Otp.Module;

public abstract class OtpController : AbpControllerBase
{
  protected OtpController()
  {
    LocalizationResource = typeof(OtpResource);
  }
}
