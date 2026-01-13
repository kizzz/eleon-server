using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ProxyRouter.Minimal.HttpApi.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyRouter.Minimal.HttpApi.Services;

public class ConfigurationLocationProvider : ILocationProvider
{
  private const string CacheKey = "ProxyRouter_Locations";
  private readonly IConfiguration _configuration;
  private readonly IMemoryCache _memoryCache;

  public ConfigurationLocationProvider(IConfiguration configuration, IMemoryCache memoryCache)
  {
    _configuration = configuration;
    _memoryCache = memoryCache;
  }

  public void Clear()
  {
    _memoryCache.Remove(CacheKey);
  }

  public Task<List<Location>> GetAsync()
  {
    var locations = _memoryCache.GetOrCreate(CacheKey, entry =>
    {
      entry.SlidingExpiration = TimeSpan.FromMinutes(30);
      return LoadLocationsFromConfiguration();
    });

    return Task.FromResult(locations ?? new List<Location>());
  }

  private List<Location> LoadLocationsFromConfiguration()
  {
    var section = _configuration.GetSection("ProxyRouter:Locations");

    if (!section.Exists())
    {
      return new List<Location>();
    }

    var locations = section.Get<List<Location>>() ?? new List<Location>();

    return locations;
  }

  public Task<ModuleSettingsDto> GetModuleProperties(string baseUrl)
  {
    return Task.FromResult(new ModuleSettingsDto { ClientApplications = [], Modules = [] });
  }
}
