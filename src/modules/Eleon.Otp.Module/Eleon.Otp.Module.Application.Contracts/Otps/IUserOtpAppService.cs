using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Otp.Module.Otps
{
  public interface IUserOtpAppService : IApplicationService
  {
    Task<bool> IgnoreLastUserOtp();
    Task<OtpGenerationResultDto> GenerateOtpAsync(string key, List<OtpRecepientEto> recipients, string message);
    Task<OtpValidationResultEto> ValidateOtpAsync(string key, string password);
  }
}
