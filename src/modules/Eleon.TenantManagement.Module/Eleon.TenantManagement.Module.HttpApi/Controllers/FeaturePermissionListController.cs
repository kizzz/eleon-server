using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.TenantManagement.Module.PermissionGroups;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/CoreInfrastructure/FeaturePermissionList")]
  public class FeaturePermissionListController : TenantManagementController, IFeaturePermissionListAppService
  {
    private readonly IFeaturePermissionListAppService appService;
    private readonly IVportalLogger<FeaturePermissionListController> logger;

    public FeaturePermissionListController(
        IFeaturePermissionListAppService appService,
        IVportalLogger<FeaturePermissionListController> logger)
    {
      this.appService = appService;
      this.logger = logger;
    }

    [HttpGet("GetAsync")]
    public async Task<FeaturePermissionListResultDto> GetAsync(string providerName, string providerKey)
    {

      var response = await appService.GetAsync(providerName, providerKey);


      return response;
    }
  }
}
