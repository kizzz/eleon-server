using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.SitesManagement.Module.Autodetect;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.Controllers
{
  [Area(SitesManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/ClientAutodetect/")]
  public class ClientAutodetectController : SitesManagementController, IClientAutodetectAppService
  {
    private readonly IClientAutodetectAppService appService;
    private readonly IVportalLogger<ClientAutodetectController> logger;

    public ClientAutodetectController(
        IClientAutodetectAppService commonUserApplicationService,
        IVportalLogger<ClientAutodetectController> logger)
    {
      this.appService = commonUserApplicationService;
      this.logger = logger;
    }

    [HttpGet]
    [Route("GetDetectedProxy")]
    public async Task<List<ApplicationModuleDto>> GetDetectedProxy(Guid proxyId)
    {

      var response = await appService.GetDetectedProxy(proxyId);


      return response;
    }

    [HttpGet]
    [Route("GetDetectedWeb")]
    public async Task<List<ApplicationModuleDto>> GetDetectedWeb(string url)
    {

      var response = await appService.GetDetectedWeb(url);


      return response;
    }
  }
}


