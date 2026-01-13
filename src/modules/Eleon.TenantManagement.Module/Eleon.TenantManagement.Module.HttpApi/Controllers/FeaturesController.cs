using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.FeatureManagement;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/Features")]
  public class FeaturesController : TenantManagementController, IFeatureAppService
  {
    private readonly IVportalLogger<FeaturesController> logger;
    private readonly IFeatureAppService featureAppService;

    public FeaturesController(
        IVportalLogger<FeaturesController> logger,
        IFeatureAppService featureAppService)
    {
      this.logger = logger;
      this.featureAppService = featureAppService;
    }

    [HttpPost("DeleteAsync")]
    public async Task DeleteAsync(string providerName, string providerKey)
    {

      await featureAppService.DeleteAsync(providerName, providerKey);

    }

    [HttpGet("GetAsync")]
    public async Task<GetFeatureListResultDto> GetAsync(string providerName, string providerKey)
    {

      var response = await featureAppService.GetAsync(providerName, providerKey);


      return response;
    }

    [HttpPost("UpdateAsync")]
    public async Task UpdateAsync(string providerName, string providerKey, UpdateFeaturesDto input)
    {

      await featureAppService.UpdateAsync(providerName, providerKey, input);

    }
  }
}
