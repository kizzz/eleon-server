using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.Microservices;
using VPortal.SitesManagement.Module.Resources;

namespace VPortal.SitesManagement.Module.Controllers
{
  [Area(SitesManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/ResourcesController")] // temp todo SitesManagement
  public class ResourceController : SitesManagementController, IResourceAppService
  {
    private readonly IVportalLogger<ResourceController> logger;
    private readonly IResourceAppService appService;

    public ResourceController(
        IVportalLogger<ResourceController> logger,
        IResourceAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("Get")]
    public async Task<EleoncoreModuleDto> GetAsync(Guid id)
    {

      var result = await appService.GetAsync(id);


      return result;
    }


    [HttpGet("GetAll")]
    public async Task<List<EleoncoreModuleDto>> GetAllAsync()
    {
      var result = await appService.GetAllAsync();
      return result;
    }

    [HttpPost("Create")]
    public async Task<EleoncoreModuleDto> CreateAsync(EleoncoreModuleDto applicationModuleDto)
    {

      var result = await appService.CreateAsync(applicationModuleDto);


      return result;
    }

    [HttpPost("Update")]
    public async Task<EleoncoreModuleDto> UpdateAsync(EleoncoreModuleDto input)
    {

      var result = await appService.UpdateAsync(input);


      return result;
    }

    [HttpDelete("Delete")]
    public async Task DeleteAsync(Guid id)
    {

      await appService.DeleteAsync(id);

    }
  }
}


