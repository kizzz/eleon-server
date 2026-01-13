using Common.EventBus.Module;
using Eleon.InternalCommons.Lib.Messages.Locations;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProxyRouter.Minimal.HttpApi.Models.Messages;
using ProxyRouter.Minimal.HttpApi.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using VPortal.Consts;
using Location = ProxyRouter.Minimal.HttpApi.Models.Messages.Location;

namespace VPortal.ProxyRouter;

public class RemoteDependency
{
  public string Url { get; set; }
  public string Expose { get; set; }
}

public class ModuleSetting
{
  public string Key { get; set; }
  public string Type { get; set; }
  public string Value { get; set; }
}
public class EleoncoreModuleConfig
{
  public List<RemoteDependency> RemoteDependencies { get; set; }
  public List<ModuleSetting> ModuleSettings { get; set; }
}

public class EventBusLocationProvider : ILocationProvider
{
  private readonly IVportalLogger<EventBusLocationProvider> logger;
  private readonly IMemoryCache memoryCache;
  private readonly IDistributedEventBus _eventBus;
  private readonly ICurrentTenant currentTenant;
  private readonly IObjectMapper mapper;
  private readonly ConfigurationLocationProvider _cfgLocationProvider;

  private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);


  public EventBusLocationProvider(
      IVportalLogger<EventBusLocationProvider> logger,
      IMemoryCache memoryCache,
      IDistributedEventBus eventBus,
      ICurrentTenant currentTenant,
      IObjectMapper mapper,
      IConfiguration configuration)
  {
    this.logger = logger;
    this.memoryCache = memoryCache;
    _eventBus = eventBus;
    this.currentTenant = currentTenant;
    this.mapper = mapper;
    _cfgLocationProvider = new ConfigurationLocationProvider(configuration, memoryCache);
  }

  public void Clear()
  {
    try
    {
      _cfgLocationProvider.Clear();
      memoryCache.Remove(ProxyRouterConsts.LocationsKey);
      memoryCache.Remove(ProxyRouterConsts.ModuleSettingsKey);
      memoryCache.Remove(ProxyRouterConsts.ModulePropertiesKey);
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
  }

  public async Task<List<Location>> GetAsync()
  {
    List<Location> result = null;
    try
    {
      var tenantId = currentTenant.Id ?? Guid.Empty;
      var cache = memoryCache.GetOrCreate(ProxyRouterConsts.LocationsKey, _ => new Dictionary<Guid, List<Location>>());

      if (cache != null && cache.TryGetValue(tenantId, out result) && result?.Count > 0)
      {
        return result;
      }

      using (await _semaphore.LockAsync())
      {
        if (cache != null && cache.TryGetValue(tenantId, out result) && result?.Count > 0)
        {
          return result;
        }

        try
        {
          var response = await _eventBus.RequestAsync<GetLocationsResponseMsg>(new GetLocationsRequestMsg { TenantId = currentTenant.Id }, 30);

          result = mapper.Map<List<LocationEto>, List<Location>>(response.Locations);
        }
        catch (Exception ex)
        {
          logger.Log.LogError(ex, "Error occurred while fetching locations");
        }

        var cfgLocations = await _cfgLocationProvider.GetAsync();
        result = LocationsMerger.MergeLocations(result ?? [], cfgLocations);

        if (cache != null)
        {
          cache[tenantId] = result;
        }
      }
    }
    catch (Exception e)
    {
      logger.Capture(e);
    }
    finally
    {
    }

    return result;
  }

  public async Task<ModuleSettingsDto> GetModuleProperties(string baseUrl)
  {
    var cache = memoryCache.GetOrCreate(ProxyRouterConsts.ModulePropertiesKey, _ => new Dictionary<Guid, ModuleSettingsDto>());

    ModuleSettingsDto result = null;

    cache?.TryGetValue(currentTenant.Id ?? Guid.Empty, out result);

    if (result != null)
    {
      return result;
    }


    var moduleSettings = await GetModuleSettings();
    var httpClient = new HttpClient();
    var moduleSettingsTasks = new List<Task>();

    foreach (var app in moduleSettings.ClientApplications)
    {

      foreach (var module in app.Modules)
      {
        var task = Task.Run(async () =>
        {
          var url = string.Empty;
          if (module.Url.StartsWith("/"))
          {
            url = baseUrl?.TrimEnd('/');
          }
          var moduleSettingsUrl = url + module.Url?.TrimEnd('/') + "/assets/eleoncore-module-settings.json";
          try
          {
            var response = await httpClient.GetAsync(moduleSettingsUrl);
            if (response.IsSuccessStatusCode)
            {
              var json = await response.Content.ReadAsStringAsync();
              var parsed = JsonConvert.DeserializeObject<EleoncoreModuleConfig>(json, new JsonSerializerSettings
              {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
              });

              var defaultSettings = parsed?.ModuleSettings?.ToDictionary(s => s.Key, s => s) ?? new Dictionary<string, ModuleSetting>();

              if (parsed?.RemoteDependencies != null && parsed.RemoteDependencies.Any())
              {
                defaultSettings["RemoteDependencies"] = new ModuleSetting
                {
                  Key = "RemoteDependencies",
                  Type = "array",
                  Value = JsonConvert.SerializeObject(parsed.RemoteDependencies, new JsonSerializerSettings
                  {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                  }),
                };
              }

              // Merge database overrides (module.Properties)
              var mergedProperties = new Dictionary<string, ModuleSetting>(defaultSettings);
              if (module.Properties != null)
              {
                foreach (var prop in module.Properties)
                {
                  if (mergedProperties.ContainsKey(prop.Key))
                  {
                    mergedProperties[prop.Key].Value = prop.Value;
                    mergedProperties[prop.Key].Type = prop.Type;
                  }
                  else
                  {
                    mergedProperties[prop.Key] = new ModuleSetting
                    {
                      Key = prop.Key,
                      Type = prop.Type,
                      Value = prop.Value
                    };
                  }
                }
              }

              // Save merged settings to module.Properties
              module.Properties = mergedProperties.Values.Select(s => new ApplicationPropertyDto
              {
                Key = s.Key,
                Value = s.Value,
                Type = s.Type,
                Level = module.Url + module.Name
              }).ToList();

            }
          }
          catch (Exception)
          {
            //logger.Log.LogWarning($"Failed to fetch default settings from {moduleSettingsUrl}: {ex.Message}");
          }
        });

        moduleSettingsTasks.Add(task);
      }
    }
    await Task.WhenAll(moduleSettingsTasks);

    if (cache != null)
    {
      cache[currentTenant.Id ?? Guid.Empty] = result;
      memoryCache.Set(ProxyRouterConsts.ModulePropertiesKey, cache);
    }

    return moduleSettings;
  }

  public async Task<ModuleSettingsDto> GetModuleSettings()
  {

    ModuleSettingsDto result = null;
    try
    {
      var cache = memoryCache.GetOrCreate(ProxyRouterConsts.ModuleSettingsKey, _ => new Dictionary<Guid, ModuleSettingsDto>());

      cache?.TryGetValue(currentTenant.Id ?? Guid.Empty, out result);

      if (result == null)
      {
        var response = await _eventBus.RequestAsync<ModuleSettingsGotMsg>(new GetModuleSettingMsg { TenantId = currentTenant.Id });
        result = mapper.Map<ModuleSettingsGotMsg, ModuleSettingsDto>(response);

        if (cache != null)
        {
          cache[currentTenant.Id ?? Guid.Empty] = result;
        }
      }
    }
    catch (Exception)
    {
      throw;
      //logger.Capture(e);
    }
    finally
    {
    }
    return result;
  }

}
