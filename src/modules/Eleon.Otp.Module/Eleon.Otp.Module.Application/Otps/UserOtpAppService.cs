using Common.Module.Keys;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VPortal.Otp.Module.DomainServices;

namespace VPortal.Otp.Module.Otps
{
  [Authorize]
  public class UserOtpAppService : OtpAppService, IUserOtpAppService
  {
    private readonly IVportalLogger<UserOtpAppService> logger;
    private readonly OtpDomainService otpDomainService;

    public UserOtpAppService(
        IVportalLogger<UserOtpAppService> logger,
        OtpDomainService otpDomainService)
    {
      this.logger = logger;
      this.otpDomainService = otpDomainService;
    }

    public async Task<bool> IgnoreLastUserOtp()
    {
      bool result = false;
      try
      {
        string key = new SignInOtpKey(CurrentUser.Id.Value).ToString();
        await otpDomainService.IgnoreLastOtp(key);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<OtpGenerationResultDto> GenerateOtpAsync(string key, List<OtpRecepientEto> recipients, string message)
    {

      OtpGenerationResultDto result = new OtpGenerationResultDto();
      try
      {
        result = await otpDomainService.GenerateOtpAsync(key, recipients, message);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<OtpValidationResultEto> ValidateOtpAsync(string key, string password)
    {

      OtpValidationResultEto result = null;
      try
      {
        result = await otpDomainService.ValidateOtp(key, password);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
