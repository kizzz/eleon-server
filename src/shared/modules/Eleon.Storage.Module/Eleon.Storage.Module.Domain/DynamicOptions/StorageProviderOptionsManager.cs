using Common.EventBus.Module;
using Commons.Module.Messages.Features;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace VPortal.Storage.Module.DynamicOptions
{
  public class StorageProviderOptionsManager : IScopedDependency
  {
    private readonly IObjectMapper objectMapper;
    private readonly IDistributedEventBus _eventBus;
    private readonly ICurrentTenant _currentTenant;

    // AsyncLocal for ambient context when called from ABP's OverrideOptionsAsync
    // This is safer than a mutable property but still allows ABP integration to work
    private static readonly AsyncLocal<string> _ambientSettingsGroup = new();

    public StorageProviderOptionsManager(
        IObjectMapper objectMapper,
        IDistributedEventBus featureSettingsAppService,
        ICurrentTenant currentTenant)
    {
      this.objectMapper = objectMapper;
      this._eventBus = featureSettingsAppService;
      _currentTenant = currentTenant;
    }

    /// <summary>
    /// Sets the ambient settings group for the current async context and returns a disposable scope.
    /// Used when calling from ABP's OverrideOptionsAsync where explicit parameter passing isn't possible.
    /// </summary>
    public static IDisposable SetAmbientSettingsGroup(string settingsGroup)
    {
      var parentScope = _ambientSettingsGroup.Value;
      _ambientSettingsGroup.Value = settingsGroup;
      return new DisposeAction(() => _ambientSettingsGroup.Value = parentScope);
    }

    /// <summary>
    /// Sets the ambient settings group for the current async context (instance method for backward compatibility).
    /// Prefer using the static SetAmbientSettingsGroup method that returns IDisposable for proper scoping.
    /// </summary>
    public void SetAmbientSettingsGroupInstance(string value)
    {
      _ambientSettingsGroup.Value = value;
    }

    /// <summary>
    /// Gets the ambient settings group from the current async context.
    /// </summary>
    public string GetAmbientSettingsGroup()
    {
      return _ambientSettingsGroup.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the explicit provider settings group string.
    /// </summary>
    public string GetExplicitProviderSettingsGroup(Guid providerId)
        => $"{StorageDomainConsts.ExplicitKeySettingGroupPrefix}{providerId}";

    /// <summary>
    /// Gets the explicit provider type settings group string.
    /// </summary>
    public string GetExplicitProviderTypeSettingsGroup(StorageTypes providerType)
        => $"{StorageDomainConsts.ExplicitProviderTypeSettingsGroup}{providerType}";

    /// <summary>
    /// Gets current storage provider settings using explicit settingsGroup parameter (preferred).
    /// Falls back to ambient context if settingsGroup is null/empty (for ABP integration).
    /// </summary>
    public async Task<KeyValuePair<string, Dictionary<string, string>>?> GetCurrentStorageProviderSettings(string settingsGroup = null)
    {
      // Use explicit parameter if provided, otherwise fall back to ambient context
      var effectiveSettingsGroup = settingsGroup ?? _ambientSettingsGroup.Value ?? string.Empty;

      string storageProviderKey;
      if (effectiveSettingsGroup.Contains(StorageDomainConsts.TestSettingGroupPrefix))
      {
        storageProviderKey = effectiveSettingsGroup;
      }
      else if (effectiveSettingsGroup.StartsWith(StorageDomainConsts.ExplicitKeySettingGroupPrefix))
      {
        storageProviderKey = effectiveSettingsGroup.Replace(StorageDomainConsts.ExplicitKeySettingGroupPrefix, string.Empty);
      }
      else if (effectiveSettingsGroup.StartsWith(StorageDomainConsts.ExplicitProviderTypeSettingsGroup))
      {
        return new(effectiveSettingsGroup.Replace(StorageDomainConsts.ExplicitProviderTypeSettingsGroup, string.Empty), new Dictionary<string, string>());
      }
      else
      {
        storageProviderKey = await GetSetting(effectiveSettingsGroup);
      }

      if (Guid.TryParse(storageProviderKey, out var storageProviderId))
      {
        var provider = await GetStorageProvider(storageProviderId);

        var settings = new Dictionary<string, string>();

        foreach (var setting in provider.Settings ?? [])
        {
          settings[setting.Key] = setting?.Value;
        }

        return new(provider.StorageProviderTypeName, settings);
      }

      return null;
    }

    private async Task<StorageProviderDto> GetStorageProvider(Guid storageProviderId)
    {
      var response = await _eventBus.RequestAsync<GetStorageProviderResponseMsg>(new GetStorageProviderMsg { StorageProviderId = storageProviderId.ToString() });
      //var storageProviderDto = await storageProvidersAppService.GetStorageProvider(storageProviderId.ToString());
      //return objectMapper.Map<StorageProviderDto, StorageProviderEntity>(storageProviderDto);
      return response.StorageProvider;
    }

    private async Task<string> GetSetting(string group, string key = null)
    {
      var response = await _eventBus.RequestAsync<GetFeatureSettingResponseMsg>(new GetFeatureSettingMsg
      {
        TenantId = _currentTenant.Id,
        Group = group,
        Key = key ?? StorageDomainConsts.StorageProviderSettingKey,
      });
      return response?.Value;
    }
  }
}
