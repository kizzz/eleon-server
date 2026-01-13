using Volo.Abp.Application.Services;
using VPortal.SitesManagement.Module.Application.Contracts.CustomFeatures;
using VPortal.SitesManagement.Module.CustomFeatures;

namespace ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.CustomFeatures;
public interface ICustomFeaturesAppService : IApplicationService
{
  Task<Dictionary<string, string>> GetSupportedValueTypesAsync();
  Task<List<CustomFeatureGroupDto>> GetAllGroupsAsync();
  Task<List<CustomFeatureDto>> GetAllFeaturesAsync();

  Task<bool> CreateBulkForMicroserviceAsync(CustomFeatureForMicroserviceDto customPermissionsForMicroserviceDto);
  Task<CustomFeatureGroupDto> CreateGroupAsync(CustomFeatureGroupDto createGroupDto);

  Task<CustomFeatureGroupDto> UpdateGroupAsync(CustomFeatureGroupDto updateGroupDto);

  Task DeleteGroupAsync(string name);

  Task<List<CustomFeatureGroupDto>> GetFeatureDynamicGroupCategoriesAsync();

  Task<CustomFeatureDto> CreateFeatureAsync(CustomFeatureDto createPermissionDto);

  Task<CustomFeatureDto> UpdateFeatureAsync(CustomFeatureDto updatePermissionDto);

  Task DeleteFeatureAsync(string name);

  Task<List<CustomFeatureDto>> GetFeaturesDynamicAsync();

  Task RequestFeaturesPermissionsUpdateAsync();

  //Task<GetFeatureListResultDto> GetAsync(string providerName, string providerKey);

  //Task UpdateAsync(string providerName, string providerKey, UpdateFeaturesDto input);

  //Task DeleteAsync(string providerName, string providerKey);
}


