using Logging.Module;
using VPortal.Core.Infrastructure.Module.Entities;
using VPortal.Core.Infrastructure.Module.FeatureSettings;
using VPortal.Infrastructure.Module;

namespace VPortal.Core.Infrastructure.Module.DataSources;

public class FeatureSettingsAppService : InfrastructureAppService, IFeatureSettingsAppService
{
  private readonly FeatureSettingDomainService _featureSettingDomainService;
  private readonly IVportalLogger<FeatureSettingsAppService> _logger;

  public FeatureSettingsAppService(FeatureSettingDomainService featureSettingDomainService, IVportalLogger<FeatureSettingsAppService> logger)
  {
    _logger = logger;
    _featureSettingDomainService = featureSettingDomainService;
  }

  public async Task<List<FeatureSettingDto>> SetAsync(List<SetFeatureSettingDto> settings)
  {

    List<FeatureSettingDto> response = new();

    try
    {
      var mappedSettings = ObjectMapper.Map<List<SetFeatureSettingDto>, List<FeatureSettingEntity>>(settings);
      List<FeatureSettingEntity> source = await _featureSettingDomainService.SetAsync(CurrentTenant.Id, mappedSettings);

      response = ObjectMapper.Map<List<FeatureSettingEntity>, List<FeatureSettingDto>>(source);
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }

    return response;
  }

  public async Task<FeatureSettingDto> GetAsync(string group, string key)
  {

    FeatureSettingDto response = null;

    try
    {
      FeatureSettingEntity source = await _featureSettingDomainService.GetAsync(CurrentTenant.Id, group, key);

      response = ObjectMapper.Map<FeatureSettingEntity, FeatureSettingDto>(source);
    }
    catch (Exception e)
    {
      _logger.Capture(e);
    }

    return response;
  }

}
