using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyRouter.Module.Services
{
  public class ConfigurationCacheService
  {
    private readonly IConfiguration configuration;
    private readonly IMemoryCache memoryCache;

    public ConfigurationCacheService(
        IConfiguration configuration,
        IMemoryCache memoryCache)
    {
      this.configuration = configuration;
      this.memoryCache = memoryCache;
    }

    public string GetSetting(string key)
    {
      return memoryCache.GetOrCreate(key, (item) => configuration[key]);
    }
    public string[] GetSettingArray(string key)
    {
      return memoryCache.GetOrCreate(key, (item) =>
      {
        return configuration.GetSection(key).Get<string[]>();
      }) ?? Array.Empty<string>();
    }
  }
}
