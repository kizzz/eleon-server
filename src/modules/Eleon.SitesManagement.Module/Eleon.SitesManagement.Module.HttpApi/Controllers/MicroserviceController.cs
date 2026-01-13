using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.Controllers
{
  [Area(SitesManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/SitesManagement/MicroserviceController")]
  public class MicroserviceController : SitesManagementController, IMicroserviceAppService
  {
    private readonly IVportalLogger<MicroserviceController> logger;
    private readonly IMicroserviceAppService appService;

    public MicroserviceController(
        IVportalLogger<MicroserviceController> logger,
        IMicroserviceAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpPost("Create")]
    public async Task<EleoncoreModuleDto> Create(EleoncoreModuleDto applicationModuleDto)
    {

      var result = await appService.Create(applicationModuleDto);


      return result;
    }

    [HttpGet("GetMicroserviceList")]
    public async Task<List<EleoncoreModuleDto>> GetMicroserviceList()
    {
      var result = await appService.GetMicroserviceList();
      return result;
    }

    [HttpPost("InitializeMicroservice")]
    public async Task<bool> InitializeMicroservice(InitializeMicroserviceMsg initializeMicroserviceMsg)
    {
      var result = await appService.InitializeMicroservice(initializeMicroserviceMsg);
      return result;
    }
  }
}


