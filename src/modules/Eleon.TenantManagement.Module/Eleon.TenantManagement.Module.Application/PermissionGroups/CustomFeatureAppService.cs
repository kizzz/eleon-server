using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.CustomFeatures;
using System.Text.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;
using VPortal.TenantManagement.Module.Application.Contracts.CustomFeatures;
using VPortal.TenantManagement.Module.CustomFeatures;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.TenantManagement.Module.FeatureGroups
{
  [Authorize]
  [ExposeServices(typeof(ICustomFeaturesAppService))]
  [Volo.Abp.DependencyInjection.Dependency(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient, ReplaceServices = true)]
  public class CustomFeatureAppService : TenantManagementAppService, ICustomFeaturesAppService
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
                { "TenantManagement::Feature:ValueTypes:Toggle", JsonSerializer.Serialize(new ToggleStringValueType()) },
                { "TenantManagement::Feature:ValueTypes:Text", JsonSerializer.Serialize(new FreeTextStringValueType()) },
                // { "TenantManagement::Feature:ValueTypes:Selection", JsonSerializer.Serialize(new SelectionStringValueType()) },
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
                  : false
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
            IsDynamic = feature.Properties.TryGetValue("Dynamic", out var dynamic) ? dynamic is bool bd ? bd : bool.Parse(dynamic.ToString()) : false
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
        var groups = ObjectMapper.Map<List<CustomFeatureGroupDto>, List<FeatureGroupDefinitionRecord>>(customFeatureForMicroserviceDto.Groups);
        foreach (var entity in groups.Select(e =>
        {
          var dto = customFeatureForMicroserviceDto.Groups.First(t => t.Name == e.Name);
          return new
          {
            Record = e,
            Dto = dto,
          };
        }))
        {
          entity.Record.ExtraProperties.TryAdd("CategoryName", entity.Dto.CategoryName);
        }
        var permissions = ObjectMapper.Map<List<CustomFeatureDto>, List<FeatureDefinitionRecord>>(customFeatureForMicroserviceDto.Features);


        result = await featuresDomainService.CreateGroupsForMicroserviceAsync(customFeatureForMicroserviceDto.SourceId, groups);
        result = await featuresDomainService.CreateFeaturesForMicroserviceAsync(customFeatureForMicroserviceDto.SourceId, permissions);
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
        createGroupDto.Id = Guid.NewGuid();
        var entity = ObjectMapper.Map<CustomFeatureGroupDto, FeatureGroupDefinitionRecord>(createGroupDto);
        entity.ExtraProperties.TryAdd("CategoryName", createGroupDto.CategoryName);
        var createdEntity = await featuresDomainService.CreateGroupAsync(entity);
        result = ObjectMapper.Map<FeatureGroupDefinitionRecord, CustomFeatureGroupDto>(createdEntity);
        result.CategoryName = (string)createdEntity.ExtraProperties.GetValueOrDefault("CategoryName", string.Empty);
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
        var entity = ObjectMapper.Map<CustomFeatureDto, FeatureDefinitionRecord>(createFeatureDto);
        var createdEntity = await featuresDomainService.CreateAsync(entity);
        result = ObjectMapper.Map<FeatureDefinitionRecord, CustomFeatureDto>(createdEntity);
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
        entities.ForEach(e => e.DisplayName = ParseLocalizedString(e.DisplayName));
        result = ObjectMapper.Map<List<FeatureGroupDefinitionRecord>, List<CustomFeatureGroupDto>>(entities);
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
        var entities = await featuresDomainService.GetFeaturesDynamic();
        entities.ForEach(entity =>
        {
          entity.DisplayName = ParseLocalizedString(entity.DisplayName);
          entity.Description = ParseLocalizedString(entity.Description);
        });
        result = ObjectMapper.Map<List<FeatureDefinitionRecord>, List<CustomFeatureDto>>(entities);
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
        var entity = ObjectMapper.Map<CustomFeatureGroupDto, FeatureGroupDefinitionRecord>(updateGroupDto);
        entity.ExtraProperties.TryAdd("CategoryName", updateGroupDto.CategoryName);
        var updatedEntity = await featuresDomainService.UpdateGroupAsync(entity);
        result = ObjectMapper.Map<FeatureGroupDefinitionRecord, CustomFeatureGroupDto>(updatedEntity);
        result.CategoryName = (string)updatedEntity.ExtraProperties.GetValueOrDefault("CategoryName", string.Empty);
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
        var entity = ObjectMapper.Map<CustomFeatureDto, FeatureDefinitionRecord>(updateFeatureDto);
        var updatedEntity = await featuresDomainService.UpdateAsync(entity);
        result = ObjectMapper.Map<FeatureDefinitionRecord, CustomFeatureDto>(updatedEntity);
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
