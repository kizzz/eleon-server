using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal;
using VPortal.Controllers;
using VPortal.GatewayManagement.Module;

namespace GatewayManagement.Module.Controllers
{

  [RemoteService(Name = VportalRemoteServiceConsts.RemoteServiceName)]
  [Route("api/connection")]
  public class HttpConnectionController : VPortalController
  {
    private readonly IVportalLogger<HttpConnectionController> logger;

    public HttpConnectionController(
        IVportalLogger<HttpConnectionController> logger)
    {
      this.logger = logger;
    }

    [HttpGet("check")]
    [AllowAnonymous]
    public async Task<StatusCodeResult> CheckHttpConnection()
    {
      return new StatusCodeResult(StatusCodes.Status200OK);
    }
  }
}
