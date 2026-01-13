using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using GatewayManagement.Module.Proxies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.GatewayManagement.Module;
using VPortal.GatewayManagement.Module.Proxies;
using VPortal.GatewayManagement.Module.StaticKeys;

namespace GatewayManagement.Module.Controllers
{
  [Area(GatewayManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = GatewayManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/GatewayManagement/GatewayStaticKey")]
  public class GatewayStaticKeyController : GatewayManagementBaseController, IGatewayStaticKeyAppService
  {
    private readonly IGatewayStaticKeyAppService appService;
    private readonly IVportalLogger<GatewayStaticKeyController> logger;

    public GatewayStaticKeyController(
        IGatewayStaticKeyAppService appService,
        IVportalLogger<GatewayStaticKeyController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpGet("GetStaticKey")]
    public async Task<string> GetStaticKey()
    {

      var response = await appService.GetStaticKey();

      return response;
    }

    [HttpPost("SetStaticKeyEnabled")]
    public async Task SetStaticKeyEnabled(bool shouldBeEnabled)
    {

      await appService.SetStaticKeyEnabled(shouldBeEnabled);

    }
  }
}
