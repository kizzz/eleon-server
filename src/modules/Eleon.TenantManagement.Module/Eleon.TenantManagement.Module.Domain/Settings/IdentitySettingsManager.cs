using Common.EventBus.Module;
using EleonsoftModuleCollector.Commons.Module.Constants.IdentitySettings;
using EleonsoftModuleCollector.Commons.Module.Messages.Identity;
using Logging.Module;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using VPortal.TenantManagement.Module.Localization;

namespace VPortal.TenantManagement.Module.Settings
{
  public class IdentitySettingsManager : IDistributedEventHandler<GetIdentitySettingsRequestMsg>, ITransientDependency
  {
    private readonly IVportalLogger<IdentitySettingsManager> logger;
    private readonly ISettingDefinitionManager settingDefinitionManager;
    private readonly ISettingProvider settingProvider;
    private readonly IStringLocalizerFactory stringLocalizerFactory;
    private readonly ISettingManager settingManager;
    private readonly IResponseContext _responseContext;

    public IStringLocalizer<TenantManagementResource> localizer { get; set; }

    public IdentitySettingsManager(
        IVportalLogger<IdentitySettingsManager> logger,
        ISettingDefinitionManager settingDefinitionManager,
        ISettingProvider settingProvider,
        IStringLocalizerFactory stringLocalizerFactory,
        ISettingManager settingManager,
        IStringLocalizer<TenantManagementResource> localizer,
        IResponseContext responseContext)
    {
      this.logger = logger;
      this.settingDefinitionManager = settingDefinitionManager;
      this.settingProvider = settingProvider;
      this.stringLocalizerFactory = stringLocalizerFactory;
      this.settingManager = settingManager;
      this.localizer = localizer;
      _responseContext = responseContext;
    }

    public async Task<List<IdentitySetting>> GetIdentitySettings()
    {
      var result = new List<IdentitySetting>();
      try
      {
        var definitions = await settingDefinitionManager.GetAllAsync();
        foreach (var (group, settingNames) in IdentitySettingConsts.SettingGroups)
        {
          foreach (var name in settingNames)
          {
            var definition = definitions.First(x => x.Name == name);
            var type = IdentitySettingConsts.SettingTypes[name];
            var value = await settingProvider.GetOrNullAsync(name);
            string displayName = null;
            if (IdentitySettingConsts.SettingNamesOverrides.TryGetValue(name, out var nameOverride))
            {
              displayName = localizer[nameOverride];
            }

            string description = null;
            if (IdentitySettingConsts.SettingDescriptionOverrides.TryGetValue(name, out var descriptionOverride))
            {
              description = localizer[descriptionOverride];
            }

            result.Add(CreateSettingFromDefinition(group, definition, type, value, displayName, description));
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SetIdentitySettings(List<IdentitySetting> settings)
    {
      try
      {
        var possibleNames = IdentitySettingConsts.SettingGroups.SelectMany(x => x.Value).ToList();
        bool allNamesValid = settings.All(x => possibleNames.Contains(x.Name));
        if (!allNamesValid)
        {
          throw new Exception("Trying to set non-Identity setting or invalid setting name.");
        }

        foreach (var setting in settings)
        {
          bool isValidType = setting.Type switch
          {
            IdentitySettingType.String => true,
            IdentitySettingType.Boolean => bool.TryParse(setting.Value, out _),
            IdentitySettingType.Number => int.TryParse(setting.Value, out _),
            _ => false
          };

          if (!isValidType)
          {
            throw new Exception("Trying to set setting with invalid type.");
          }
        }

        foreach (var setting in settings)
        {
          await settingManager.SetForCurrentTenantAsync(setting.Name, setting.Value);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private IdentitySetting CreateSettingFromDefinition(string group, SettingDefinition definition, IdentitySettingType type, string value, string nameOverride, string descriptionOverride)
        => new()
        {
          Name = definition.Name,
          GroupName = group,
          DisplayName = nameOverride ?? definition.DisplayName.Localize(stringLocalizerFactory),
          Description = descriptionOverride ?? definition.Description.Localize(stringLocalizerFactory),
          Type = type,
          Value = value
        };

    public async Task HandleEventAsync(GetIdentitySettingsRequestMsg eventData)
    {
      try
      {
        var result = await GetIdentitySettings();
        await _responseContext.RespondAsync(new IdentitySettingsResponseMsg
        {
          Settings = result
        });
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }
    }
  }
}
