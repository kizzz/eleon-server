using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GatewayManagement.Module.Proxies;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.GatewayManagement.Module;
using VPortal.GatewayManagement.Module.Proxies;

namespace GatewayManagement.Module.Controllers
{
  [Area(GatewayManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = GatewayManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/GatewayManagement/GatewayClient")]
  public class GatewayClientController : GatewayManagementBaseController, IGatewayClientAppService
  {
    private readonly IGatewayClientAppService appService;
    private readonly IVportalLogger<GatewayClientController> logger;

    public GatewayClientController(
        IGatewayClientAppService appService,
        IVportalLogger<GatewayClientController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpPost("ConfirmGatewayRegistration")]
    public async Task<bool> ConfirmGatewayRegistration()
    {

      var response = await appService.ConfirmGatewayRegistration();

      return response;
    }

    [HttpGet("GetCurrentGateway")]
    public async Task<GatewayDto> GetCurrentGateway()
    {

      var response = await appService.GetCurrentGateway();

      return response;
    }

    [HttpGet("GetCurrentGatewayWorkspace")]
    public async Task<GatewayWorkspaceDto> GetCurrentGatewayWorkspace(string workspaceName)
    {

      var response = await appService.GetCurrentGatewayWorkspace(workspaceName);

      return response;
    }

    [HttpPost("RegisterGateway")]
    [AllowAnonymous]
    public async Task<GatewayRegistrationResultDto> RegisterGateway([FromBody] RegisterGatewayRequestDto requestDto)
    {

      var response = await appService.RegisterGateway(requestDto);

      return response;
    }

    [HttpPost("SetGatewayHealthStatus")]
    public async Task SetGatewayHealthStatus(SetGatewayHealthStatusRequestDto request)
    {

      await appService.SetGatewayHealthStatus(request);

    }
  }
}
