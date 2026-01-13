using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using VPortal.Core.Infrastructure.Module.FeatureSettings;
using VPortal.Infrastructure.Module;

namespace Core.Infrastructure.Module.Controllers
{
  [Area(InfrastructureRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = InfrastructureRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Infrastructure/FeatureSettings/")]
  public class FeatureSettingsController : InfrastructureController, IFeatureSettingsAppService
  {
    private readonly IFeatureSettingsAppService featureSettings;
    private readonly IVportalLogger<FeatureSettingsController> _logger;

    public FeatureSettingsController(
        IFeatureSettingsAppService featureSettings,
        IVportalLogger<FeatureSettingsController> logger)
    {
      this.featureSettings = featureSettings;
      _logger = logger;
    }

    [HttpGet]
    [Route("Get")]
    public async Task<FeatureSettingDto> GetAsync(string group, string key)
    {

      var response = await featureSettings.GetAsync(group, key);


      return response;
    }

    [HttpPost]
    [Route("Set")]
    public async Task<List<FeatureSettingDto>> SetAsync(List<SetFeatureSettingDto> settings)
    {

      var response = await featureSettings.SetAsync(settings);


      return response;
    }
  }
}
