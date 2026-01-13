using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.Autodetect;
using VPortal.SitesManagement.Module.Microservices;
using VPortal.TenantManagement.Module.Autodetect;

namespace VPortal.SitesManagement.Module.Controllers
{
  [Area(SitesManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/ServersideAutodetect/")]
  public class ServersideAutodetectController : SitesManagementController, IServersideAutodetectAppService
  {
    private readonly IServersideAutodetectAppService appService;
    private readonly IVportalLogger<ServersideAutodetectController> logger;

    public ServersideAutodetectController(
        IServersideAutodetectAppService commonUserApplicationService,
        IVportalLogger<ServersideAutodetectController> logger)
    {
      this.appService = commonUserApplicationService;
      this.logger = logger;
    }

    [HttpGet]
    [Route("GetDetectedModules")]
    public async Task<List<ApplicationModuleDto>> GetDetectedModules()
    {

      var response = await appService.GetDetectedModules();


      return response;
    }

    [HttpGet]
    [Route("StartDetect")]
    public async Task StartDetect()
    {

      await appService.StartDetect();


    }
  }
}


