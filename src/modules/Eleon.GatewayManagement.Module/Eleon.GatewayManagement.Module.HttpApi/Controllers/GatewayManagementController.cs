using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using GatewayManagement.Module.Proxies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.GatewayManagement.Module;
using VPortal.GatewayManagement.Module.Proxies;

namespace GatewayManagement.Module.Controllers
{
  [Area(GatewayManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = GatewayManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/GatewayManagement/GatewayManagement")]
  public class GatewayManagementController : GatewayManagementBaseController, IGatewayManagementAppService
  {
    private readonly IGatewayManagementAppService appService;
    private readonly IVportalLogger<GatewayManagementController> logger;

    public GatewayManagementController(
        IGatewayManagementAppService appService,
        IVportalLogger<GatewayManagementController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpPost("AcceptPendingGateway")]
    public async Task AcceptPendingGateway(AcceptPendingGatewayRequestDto request)
    {

      await appService.AcceptPendingGateway(request);

    }

    [HttpPost("AddGateway")]
    public async Task<string> AddGateway(GatewayDto gateway)
    {

      var response = await appService.AddGateway(gateway);

      return response;
    }

    [HttpPost("CancelOngoingGatewayRegistration")]
    public async Task<bool> CancelOngoingGatewayRegistration(Guid gatewayId)
    {

      var response = await appService.CancelOngoingGatewayRegistration(gatewayId);

      return response;
    }

    [HttpGet("GetCurrentGatewayRegistrationKey")]
    public async Task<GatewayRegistrationKeyDto> GetCurrentGatewayRegistrationKey(Guid gatewayId)
    {

      var response = await appService.GetCurrentGatewayRegistrationKey(gatewayId);

      return response;
    }

    [HttpGet("GetGateway")]
    public async Task<GatewayDto> GetGateway(Guid gatewayId)
    {

      var response = await appService.GetGateway(gatewayId);

      return response;
    }

    [HttpGet("GetGatewayList")]
    public async Task<List<GatewayDto>> GetGatewayList(GatewayListRequestDto request)
    {

      var response = await appService.GetGatewayList(request);

      return response;
    }

    [HttpPost("RejectPendingGateway")]
    public async Task RejectPendingGateway(Guid gatewayId)
    {

      await appService.RejectPendingGateway(gatewayId);

    }

    [HttpPost("RemoveGateway")]
    public async Task<bool> RemoveGateway(Guid gatewayId)
    {

      var response = await appService.RemoveGateway(gatewayId);

      return response;
    }

    [HttpPost("RequestGatewayRegistration")]
    public async Task<GatewayRegistrationKeyDto> RequestGatewayRegistration(Guid gatewayId)
    {

      var response = await appService.RequestGatewayRegistration(gatewayId);

      return response;
    }

    [HttpPost("UpdateGateway")]
    public async Task<bool> UpdateGateway(UpdateGatewayRequestDto gateway)
    {

      var response = await appService.UpdateGateway(gateway);

      return response;
    }
  }
}
