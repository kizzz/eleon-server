using AutoMapper;
using Eleoncore.SDK.Helpers;
using EleoncoreProxy.Api;
using EleoncoreProxy.Model;
using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using EleonsoftSdk.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.Managers.Locations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProxyRouter.Minimal.HttpApi.Models.Constants;
using ProxyRouter.Minimal.HttpApi.Models.Messages;
using ProxyRouter.Minimal.HttpApi.Services;
using System.Web;
using VPortal.Consts;

using Location = ProxyRouter.Minimal.HttpApi.Models.Messages.Location;

namespace ProxyRouter.Minimal.Host;
public class RemoteDependency
{
  public required string Url { get; set; }
  public required string Expose { get; set; }
}

public class ModuleSetting
{
  public required string Key { get; set; }
  public required string Type { get; set; }
  public required string Value { get; set; }
}
public class EleoncoreModuleConfig
{
  public required List<RemoteDependency> RemoteDependencies { get; set; }
  public required List<ModuleSetting> ModuleSettings { get; set; }
}
public class SdkLocationProvider : ILocationProvider
{
  private readonly IMemoryCache memoryCache;
  private readonly IHttpContextAccessor httpContextAccessor;
  private readonly IClientApplicationApi clientApplicationApi;
  private readonly IMapper mapper;
  private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10); // Cache duration
  private readonly ConfigurationLocationProvider _cfgLocationProvider;

  public SdkLocationProvider(
      IMemoryCache memoryCache,
      IHttpContextAccessor httpContextAccessor,
      IClientApplicationApi clientApplicationApi,
      IMapper mapper,
      IConfiguration configuration)
  {
    this.memoryCache = memoryCache;
    this.httpContextAccessor = httpContextAccessor;
    this.clientApplicationApi = clientApplicationApi;
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
    catch (Exception)
    {
      //logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task<List<Location>> GetAsync()
  {
    List<Location> result = null;
    try
    {
      if (httpContextAccessor.HttpContext == null)
      {
        return null;
      }

      var host = EleonsoftTenantHelper.ExtractHostnameFromContext(httpContextAccessor.HttpContext);

      var cache = memoryCache.GetOrCreate(ProxyRouterConsts.LocationsKey, _ => new Dictionary<string, List<Location>>());

      cache?.TryGetValue(host, out result);

      if (result == null)
      {
        clientApplicationApi.UseApiAuth();
        var response = await clientApplicationApi.SitesManagementClientApplicationGetLocationsAsync();

        var data = response.Ok();

        if (data == null)
        {
          throw new Exception($"Failed to retrieve locations from SDK. Status code: {response.StatusCode}");
        }

        result = new List<Location>();
        foreach (var location in data)
        {
          result.Add(ToLocation(location));
        }

        var cfgLocations = await _cfgLocationProvider.GetAsync();
        result = LocationsMerger.MergeLocations(result, cfgLocations);

        if (cache != null)
        {
          cache[host] = result;
        }
      }

      AddNewAuthorityIfNotExists(host);
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


  private static Location ToLocation(ModuleCollectorLocation location)
  {
    return new Location
    {
      Path = location.Path,
      Type = (LocationType)location.Type,
      SourceUrl = location.SourceUrl,
      RemotePath = string.Empty,
      CheckedPath = string.Empty,
      DefaultRedirect = location.DefaultRedirect,
      ResourceId = location.ResourceId,
      IsAuthorized = location.IsAuthorized ?? false,
      RequiredPolicy = location.RequiredPolicy,
      SubLocations = location.SubLocations?.Where(x => x != null).Select(ToLocation).ToList() ?? [],
    };
  }
  public void AddNewAuthorityIfNotExists(string authority)
  {
    var authorities = memoryCache.GetOrCreate<List<string>>("Authorities", (_) => new List<string>() { });

    if (!authorities.Contains(authority))
    {
      authorities.Add(authority);
      memoryCache.Set("Authorities", authorities, CacheExpiration);
    }
    EleoncoreIssuerValidatorHelper.Authorities = authorities;
  }

  public async Task<ModuleSettingsDto> GetModuleSettings()
  {
    ModuleSettingsDto result = null;
    try
    {
      if (httpContextAccessor.HttpContext == null)
      {
        return null;
      }

      var host = EleonsoftTenantHelper.ExtractHostnameFromContext(httpContextAccessor.HttpContext);

      var cache = memoryCache.GetOrCreate(ProxyRouterConsts.ModuleSettingsKey, _ => new Dictionary<string, ModuleSettingsDto>());

      cache?.TryGetValue(host, out result);

      if (result == null)
      {
        var response = await clientApplicationApi.SitesManagementClientApplicationGetSettingAsync();
        result = mapper.Map<ModuleSettingsDto>(response.Ok());

        if (cache != null)
        {
          cache[host] = result;
        }
      }

      AddNewAuthorityIfNotExists(host);
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

  public async Task<ModuleSettingsDto> GetModuleProperties(string baseUrl)
  {
    var host = EleonsoftTenantHelper.ExtractHostnameFromContext(httpContextAccessor.HttpContext);
    var cache = memoryCache.GetOrCreate(ProxyRouterConsts.ModulePropertiesKey, _ => new Dictionary<string, ModuleSettingsDto>());

    ModuleSettingsDto result = null;

    cache?.TryGetValue(host, out result);

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
              var parsed = JsonConvert.DeserializeObject<EleoncoreModuleConfig>(
                  json,
                  new JsonSerializerSettings
                  {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
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
      cache[host] = result;
      memoryCache.Set(ProxyRouterConsts.ModulePropertiesKey, cache);
    }

    return moduleSettings;
  }
}
