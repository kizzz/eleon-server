using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using GatewayManagement.Module.Proxies;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Content;
using VPortal.GatewayManagement.Module;

namespace GatewayManagement.Module.Controllers
{
  [Area(GatewayManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = GatewayManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/GatewayManagement/GatewayHttpForwarding")]
  public class GatewayHttpForwardingController : GatewayManagementBaseController, IGatewayHttpForwardingAppService
  {
    private readonly IGatewayHttpForwardingAppService appService;
    private readonly IVportalLogger<GatewayHttpForwardingController> logger;

    public GatewayHttpForwardingController(
        IGatewayHttpForwardingAppService appService,
        IVportalLogger<GatewayHttpForwardingController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpGet("GetForwardedRequest")]
    public async Task<IRemoteStreamContent> GetForwardedRequest(Guid requestId)
    {
      var response = await appService.GetForwardedRequest(requestId);
      return response;
    }

    [HttpPost("SendForwardedResponse")]
    public async Task<bool> SendForwardedResponse(GatewayForwardedResponseDto response)
    {
      var result = await appService.SendForwardedResponse(response);
      return result;
    }
  }
}
