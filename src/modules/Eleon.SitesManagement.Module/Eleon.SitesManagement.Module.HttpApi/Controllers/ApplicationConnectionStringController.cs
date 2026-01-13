using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.ApplicationConnectionStrings;

namespace VPortal.SitesManagement.Module.Controllers
{
  [Area(SitesManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/SitesManagement/ApplicationConnectionString")]
  public class ApplicationConnectionStringController : SitesManagementController, IApplicationConnectionStringAppService
  {
    private readonly IVportalLogger<ApplicationConnectionStringController> logger;
    private readonly IApplicationConnectionStringAppService connectionStringAppService;

    public ApplicationConnectionStringController(
        IVportalLogger<ApplicationConnectionStringController> logger,
        IApplicationConnectionStringAppService connectionStringAppService)
    {
      this.logger = logger;
      this.connectionStringAppService = connectionStringAppService;
    }

    [HttpPost("AddConnectionString")]
    public async Task<bool> AddConnectionString(CreateConnectionStringRequestDto request)
    {

      var response = await connectionStringAppService.AddConnectionString(request);


      return response;
    }

    [HttpGet("Get")]
    public async Task<ConnectionStringDto> GetAsync(Guid? tenantId, string applicationName)
    {

      var response = await connectionStringAppService.GetAsync(tenantId, applicationName);


      return response;
    }
    [HttpPost("SetConnectionString")]
    public async Task SetConnectionStringAsync(SetConnectionStringRequestDto request)
    {

      await connectionStringAppService.SetConnectionStringAsync(request);

    }

    [HttpGet("GetConnectionStrings")]
    public async Task<List<ConnectionStringDto>> GetConnectionStrings(Guid tenantId)
    {

      var response = await connectionStringAppService.GetConnectionStrings(tenantId);


      return response;
    }



    [HttpPost("RemoveConnectionString")]
    public async Task<bool> RemoveConnectionString(RemoveConnectionStringRequestDto request)
    {

      var response = await connectionStringAppService.RemoveConnectionString(request);


      return response;
    }

    [HttpPost("UpdateConnectionString")]
    public async Task<bool> UpdateConnectionString(UpdateConnectionStringRequestDto request)
    {

      var response = await connectionStringAppService.UpdateConnectionString(request);


      return response;
    }
  }
}


