using Eleon.AbpSdk.Lib.modules.HostExtensions.Module.Auth;
using EleonsoftAbp.Auth;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.CustomFeatures;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.DomainServices;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain;
using System.Text.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;
using VPortal.SitesManagement.Module.Application.Contracts.CustomFeatures;
using VPortal.SitesManagement.Module.CustomFeatures;

namespace VPortal.SitesManagement.Module.FeatureGroups
{
  [Authorize]
  [ExposeServices(typeof(ICustomFeaturesAppService))]
  [Volo.Abp.DependencyInjection.Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
  public class CustomFeatureAppService : SitesManagementAppService, ICustomFeaturesAppService
  {
    private readonly IVportalLogger<CustomFeatureAppService> logger;
    private readonly CustomFeaturesDomainService featuresDomainService;
    private readonly IFeatureDefinitionManager featureDefinitionManager;
    private readonly IStringLocalizerFactory localizerFactory;
    private readonly IDistributedEventBus bus;

    public CustomFeatureAppService(
        IVportalLogger<CustomFeatureAppService> logger,
        CustomFeaturesDomainService featuresDomainService,
        IFeatureDefinitionManager featureDefinitionManager,
        IStringLocalizerFactory localizerFactory,
        IDistributedEventBus bus
        )
    {
      this.logger = logger;
      this.featuresDomainService = featuresDomainService;
      this.featureDefinitionManager = featureDefinitionManager;
      this.localizerFactory = localizerFactory;
      this.bus = bus;
    }

    public Task<Dictionary<string, string>> GetSupportedValueTypesAsync()
    {
      return Task.FromResult(new Dictionary<string, string>()
            {
                { "SitesManagement::Feature:ValueTypes:Toggle", JsonSerializer.Serialize(new ToggleStringValueType()) },
                { "SitesManagement::Feature:ValueTypes:Text", JsonSerializer.Serialize(new FreeTextStringValueType()) },
                // { "SitesManagement::Feature:ValueTypes:Selection", JsonSerializer.Serialize(new SelectionStringValueType()) },
            });
    }

    public async Task<List<CustomFeatureGroupDto>> GetAllGroupsAsync()
    {
      List<CustomFeatureGroupDto> result = new List<CustomFeatureGroupDto>();
      try
      {
        var dynamics = await GetFeatureDynamicGroupCategoriesAsync();
        result.AddRange(dynamics);

        // add static features
        var statics = (await featureDefinitionManager.GetGroupsAsync())
            .Where(sg => dynamics.FirstOrDefault(dg => dg.Name == sg.Name) == null);

        foreach (var staticGroup in statics)
        {
          result.Add(new CustomFeatureGroupDto()
          {
            Id = Guid.Empty,
            Name = staticGroup.Name,
            DisplayName = ParseLocalizedString(staticGroup.DisplayName),
            CategoryName = staticGroup.Properties.TryGetValue("CategoryName", out var category) ? category.ToString() : null,
            IsDynamic = staticGroup.Properties.TryGetValue("Dynamic", out var dynamic) ?
                  dynamic is bool bd ?
                      bd
                      : bool.TryParse(dynamic.ToString(), out var res) ?
                          res
                          : false
                  : false,
          });
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private string ParseLocalizedString(ILocalizableString localizableString)
    {
      if (localizableString == null)
      {
        return string.Empty;
      }
      string result = string.Empty;
      try
      {
        var localized = localizableString.Localize(localizerFactory);
        result = localized.Value;
      }
      catch
      {
        try
        {
          if (localizableString is FixedLocalizableString fixedLocalizable)
          {
            result = fixedLocalizable.Value;
          }
          else
          {
            result = localizableString.ToString();
          }
        }
        catch { }
      }
      return result;
    }
    private string ParseLocalizedString(string localizableString)
    {
      if (localizableString == null)
      {
        return string.Empty;
      }
      string result = string.Empty;
      try
      {
        if (localizableString.Length < 2)
        {
          result = localizableString;
        }
        else if (localizableString.StartsWith("L:"))
        {
          var lString = localizableString.Substring(2);
          var parts = lString.Split(new char[] { ',' }, 2);
          if (parts.Length == 2)
          {
            var localizer = localizerFactory.CreateByResourceName(parts[0]);
            result = localizer[parts[1]];
          }
          else
          {
            result = lString;
          }
        }
        else if (localizableString.StartsWith("F:"))
        {
          result = localizableString.Substring(2);
        }
        else
        {
          result = localizableString;
        }
      }
      catch
      {

      }
      return result;
    }

    public async Task<List<CustomFeatureDto>> GetAllFeaturesAsync()
    {
      List<CustomFeatureDto> result = new List<CustomFeatureDto>();
      try
      {
        var dynamics = await GetFeaturesDynamicAsync();
        result.AddRange(dynamics);

        var statics = await featureDefinitionManager.GetGroupsAsync();
        var staticsList = new List<CustomFeatureDto>();
        foreach (var entity in statics)
        {
          AddFeatures(staticsList, entity.Features, entity.Name);
        }
        result.AddRange(staticsList.Where(sf => dynamics.FirstOrDefault(df => df.Name == sf.Name) == null));
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;

      void AddFeatures(List<CustomFeatureDto> source, IEnumerable<FeatureDefinition> features, string groupName)
      {
        foreach (var feature in features)
        {
          source.Add(new CustomFeatureDto()
          {
            Id = Guid.Empty,
            Name = feature.Name,
            DisplayName = ParseLocalizedString(feature.DisplayName),
            DefaultValue = feature.DefaultValue,
            GroupName = groupName,
            ParentName = feature.Parent?.Name,
            Description = ParseLocalizedString(feature.Description),
            IsVisibleToClients = feature.IsVisibleToClients,
            IsAvailableToHost = feature.IsAvailableToHost,
            AllowedProviders = feature.AllowedProviders?.ToString(),
            ValueType = JsonSerializer.Serialize(feature.ValueType),
            IsDynamic = feature.Properties.TryGetValue("Dynamic", out var dynamic) ? dynamic is bool bd ? bd : bool.Parse(dynamic.ToString()) : false,
          });
          AddFeatures(source, feature.Children, groupName);
        }
      }
    }

    public async Task<bool> CreateBulkForMicroserviceAsync(CustomFeatureForMicroserviceDto customFeatureForMicroserviceDto)
    {
      bool result = false;
      try
      {
        var apiKeyIdentifier = CurrentUser.GetApiKeyName();

        if (string.IsNullOrEmpty(apiKeyIdentifier))
        {
          throw new Exception("API Key identifier is required to create custom permissions for a microservice.");
        }

        var groups = ObjectMapper.Map<List<CustomFeatureGroupDto>, List<FeatureGroupDefinitionEto>>(customFeatureForMicroserviceDto.Groups);
        var permissions = ObjectMapper.Map<List<CustomFeatureDto>, List<FeatureDefinitionEto>>(customFeatureForMicroserviceDto.Features);

        result = await featuresDomainService.CreateBulkForMicroserviceAsync(apiKeyIdentifier, groups, permissions);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<CustomFeatureGroupDto> CreateGroupAsync(CustomFeatureGroupDto createGroupDto)
    {
      CustomFeatureGroupDto result = null;
      try
      {
        var entity = ObjectMapper.Map<CustomFeatureGroupDto, FeatureGroupDefinitionEto>(createGroupDto);
        var createdEntity = await featuresDomainService.CreateGroupAsync(entity);
        result = ObjectMapper.Map<FeatureGroupDefinitionEto, CustomFeatureGroupDto>(createdEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<CustomFeatureDto> CreateFeatureAsync(CustomFeatureDto createFeatureDto)
    {
      CustomFeatureDto result = null;
      try
      {
        createFeatureDto.Id = Guid.NewGuid();
        var entity = ObjectMapper.Map<CustomFeatureDto, FeatureDefinitionEto>(createFeatureDto);
        var createdEntity = await featuresDomainService.CreateAsync(entity);
        result = ObjectMapper.Map<FeatureDefinitionEto, CustomFeatureDto>(createdEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task DeleteGroupAsync(string name)
    {
      try
      {
        await featuresDomainService.DeleteGroupAsync(name);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task DeleteFeatureAsync(string name)
    {
      try
      {
        await featuresDomainService.DeleteAsync(name);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<CustomFeatureGroupDto>> GetFeatureDynamicGroupCategoriesAsync()
    {
      List<CustomFeatureGroupDto> result = null;
      try
      {
        var entities = await featuresDomainService.GetFeatureDynamicGroupCategories();
        result = ObjectMapper.Map<List<FeatureGroupDefinitionEto>, List<CustomFeatureGroupDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<CustomFeatureDto>> GetFeaturesDynamicAsync()
    {
      List<CustomFeatureDto> result = null;
      try
      {
        var entities = await featuresDomainService.GetFeaturesAsync();
        result = ObjectMapper.Map<List<FeatureDefinitionEto>, List<CustomFeatureDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<CustomFeatureGroupDto> UpdateGroupAsync(CustomFeatureGroupDto updateGroupDto)
    {
      CustomFeatureGroupDto result = null;
      try
      {
        var entity = ObjectMapper.Map<CustomFeatureGroupDto, FeatureGroupDefinitionEto>(updateGroupDto);
        var updatedEntity = await featuresDomainService.UpdateGroupAsync(entity);
        result = ObjectMapper.Map<FeatureGroupDefinitionEto, CustomFeatureGroupDto>(updatedEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<CustomFeatureDto> UpdateFeatureAsync(CustomFeatureDto updateFeatureDto)
    {
      CustomFeatureDto result = null;
      try
      {
        var entity = ObjectMapper.Map<CustomFeatureDto, FeatureDefinitionEto>(updateFeatureDto);
        var updatedEntity = await featuresDomainService.UpdateAsync(entity);
        result = ObjectMapper.Map<FeatureDefinitionEto, CustomFeatureDto>(updatedEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task RequestFeaturesPermissionsUpdateAsync()
    {
      await bus.PublishAsync(new UpdateFeaturesPermissionsRequestMsg());
    }
  }
}


