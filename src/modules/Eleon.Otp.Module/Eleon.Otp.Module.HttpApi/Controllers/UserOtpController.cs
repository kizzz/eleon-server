using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Otp.Module.Otps;

namespace VPortal.Otp.Module.Controllers
{
  [Area(OtpRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = OtpRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Otp/UserOtp")]
  public class UserOtpController : OtpController, IUserOtpAppService
  {
    private readonly IVportalLogger<UserOtpController> logger;
    private readonly IUserOtpAppService appService;

    public UserOtpController(
        IVportalLogger<UserOtpController> logger,
        IUserOtpAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpPost("IgnoreLastUserOtp")]
    public async Task<bool> IgnoreLastUserOtp()
    {

      var response = await appService.IgnoreLastUserOtp();

      return response;
    }

    [HttpPost("Generate")]
    public async Task<OtpGenerationResultDto> GenerateOtpAsync(string key, List<OtpRecepientEto> recipients, string message)
    {

      var response = await appService.GenerateOtpAsync(key, recipients, message);


      return response;
    }

    [HttpGet("Validate")]
    public async Task<OtpValidationResultEto> ValidateOtpAsync(string key, string password)
    {

      var response = await appService.ValidateOtpAsync(key, password);


      return response;
    }
  }
}
