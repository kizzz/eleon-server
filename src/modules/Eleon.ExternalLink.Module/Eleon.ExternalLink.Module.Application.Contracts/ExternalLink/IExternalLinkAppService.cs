using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.ExternalLink.Module.FileExternalLink
{
  public interface IExternalLinkAppService : IApplicationService
  {
    public Task<ExternalLinkLoginInfoDto> GetLoginInfoAsync(string code);
    public Task<string> DirectLoginAsync(string code, string password);
    public Task<string> LoginWithOtp(string linkCode, string otp);
    public Task<string> GetOtp(string linkCode);
  }
}
