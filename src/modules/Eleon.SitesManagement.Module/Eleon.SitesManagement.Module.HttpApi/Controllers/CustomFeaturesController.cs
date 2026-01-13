using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.CustomFeatures;
using Volo.Abp;
using VPortal.SitesManagement.Module.Application.Contracts.CustomFeatures;
using VPortal.SitesManagement.Module.CustomFeatures;

namespace VPortal.SitesManagement.Module.Controllers;

[Area(SitesManagementRemoteServiceConsts.ModuleName)]
[RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
[Route("api/SitesManagement/CustomFeatures")]
public class CustomFeaturesController : SitesManagementController, ICustomFeaturesAppService
{
  private readonly IVportalLogger<CustomFeaturesController> logger;
  private readonly ICustomFeaturesAppService featuresAppService;

  public CustomFeaturesController(
      IVportalLogger<CustomFeaturesController> logger,
      ICustomFeaturesAppService featuresAppService)
  {
    this.logger = logger;
    this.featuresAppService = featuresAppService;
  }

  [HttpGet("GetSupportedValueTypes")]
  public async Task<Dictionary<string, string>> GetSupportedValueTypesAsync()
  {

    var response = await featuresAppService.GetSupportedValueTypesAsync();


    return response;
  }

  [HttpGet("GetAllGroups")]
  public async Task<List<CustomFeatureGroupDto>> GetAllGroupsAsync()
  {

    var response = await featuresAppService.GetAllGroupsAsync();


    return response;
  }

  [HttpGet("GetAllFeatures")]
  public async Task<List<CustomFeatureDto>> GetAllFeaturesAsync()
  {

    var response = await featuresAppService.GetAllFeaturesAsync();


    return response;
  }

  [HttpPost("CreateGroup")]
  public async Task<CustomFeatureGroupDto> CreateGroupAsync(CustomFeatureGroupDto createGroupDto)
  {

    var response = await featuresAppService.CreateGroupAsync(createGroupDto);

    return response;
  }

  [HttpPut("UpdateGroup")]
  public async Task<CustomFeatureGroupDto> UpdateGroupAsync(CustomFeatureGroupDto updateGroupDto)
  {

    var response = await featuresAppService.UpdateGroupAsync(updateGroupDto);

    return response;
  }

  [HttpDelete("DeleteGroup")]
  public async Task DeleteGroupAsync(string name)
  {

    await featuresAppService.DeleteGroupAsync(name);

  }

  [HttpGet("GetFeatureGroups")]
  public async Task<List<CustomFeatureGroupDto>> GetFeatureDynamicGroupCategoriesAsync()
  {

    var response = await featuresAppService.GetFeatureDynamicGroupCategoriesAsync();

    return response;
  }

  [HttpPost("CreateFeature")]
  public async Task<CustomFeatureDto> CreateFeatureAsync(CustomFeatureDto createFeatureDto)
  {

    var response = await featuresAppService.CreateFeatureAsync(createFeatureDto);

    return response;
  }

  [HttpPut("UpdateFeature")]
  public async Task<CustomFeatureDto> UpdateFeatureAsync(CustomFeatureDto updateFeatureDto)
  {

    var response = await featuresAppService.UpdateFeatureAsync(updateFeatureDto);

    return response;
  }

  [HttpDelete("DeleteFeature")]
  public async Task DeleteFeatureAsync(string name)
  {

    await featuresAppService.DeleteFeatureAsync(name);

  }

  [HttpGet("GetFeatures")]
  public async Task<List<CustomFeatureDto>> GetFeaturesDynamicAsync()
  {

    var response = await featuresAppService.GetFeaturesDynamicAsync();

    return response;
  }

  [HttpPost("CreateBulkForMicroserviceAsync")]
  public async Task<bool> CreateBulkForMicroserviceAsync(CustomFeatureForMicroserviceDto customFeaturesForMicroserviceDto)
  {

    var response = await featuresAppService.CreateBulkForMicroserviceAsync(customFeaturesForMicroserviceDto);

    return response;
  }

  [HttpPost("RequestFeaturesPermissionsUpdate")]
  public async Task RequestFeaturesPermissionsUpdateAsync()
  {

    await featuresAppService.RequestFeaturesPermissionsUpdateAsync();

  }
}


