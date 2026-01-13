using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.ExternalLink.Module.FileExternalLink;

namespace VPortal.ExternalLink.Module.Controllers
{
  [Area(ExternalLinkRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ExternalLinkRemoteServiceConsts.RemoteServiceName)]
  [Route("api/external-links")]
  public class ExternalLinkController : ModuleController, IExternalLinkAppService
  {
    private readonly IVportalLogger<ExternalLinkController> _logger;
    private readonly IExternalLinkAppService _appService;

    public ExternalLinkController(
        IVportalLogger<ExternalLinkController> logger,
        IExternalLinkAppService appService)
    {
      _logger = logger;
      _appService = appService;
    }
    [HttpPost("DirectLoginAsync")]
    public async Task<string> DirectLoginAsync(string code, string password)
    {

      var response = await _appService.DirectLoginAsync(code, password);


      return response;
    }

    [HttpPost("GetLoginInfoAsync")]
    public async Task<ExternalLinkLoginInfoDto> GetLoginInfoAsync(string code)
    {

      var response = await _appService.GetLoginInfoAsync(code);


      return response;
    }

    [HttpPost("GetOtp")]
    public async Task<string> GetOtp(string linkCode)
    {

      var response = await _appService.GetOtp(linkCode);


      return response;

    }

    [HttpPost("LoginWithOtp")]
    public async Task<string> LoginWithOtp(string linkCode, string otp)
    {

      var response = await _appService.LoginWithOtp(linkCode, otp);


      return response;
    }
  }
}
