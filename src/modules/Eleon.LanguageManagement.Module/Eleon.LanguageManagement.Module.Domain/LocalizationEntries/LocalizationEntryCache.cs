using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace VPortal.LanguageManagement.Module.LocalizationEntries
{
  public class LocalizationEntryCache : ISingletonDependency
  {
    private readonly ConcurrentDictionary<string, StaticLocalizationDictionary> cache = new();
    private readonly ICurrentTenant currentTenant;
    private readonly SemaphoreSlim semaphore = new(1, 1);

    public LocalizationEntryCache(ICurrentTenant currentTenant)
    {
      this.currentTenant = currentTenant;
    }

    public async Task<StaticLocalizationDictionary> GetOrCreateLocalizationDictionary(
        string culture,
        string resource,
        Func<Task<StaticLocalizationDictionary>> factory)
    {
      using (await semaphore.LockAsync())
      {
        if (cache.TryGetValue(GetCultureResourceCacheKey(culture, resource), out var dictionary))
        {
          return dictionary;
        }
        else
        {
          var newDictionary = await factory();
          cache[GetCultureResourceCacheKey(culture, resource)] = newDictionary;
          return newDictionary;
        }
      }
    }

    public void ResetLocalizationDictionary(string culture, string resource)
    {
      string key = GetCultureResourceCacheKey(culture, resource);
      cache.TryRemove(key, out _);
    }

    public StaticLocalizationDictionary GetLocalizationDictionaryOrNull(string culture, string resource)
    {
      string key = GetCultureResourceCacheKey(culture, resource);
      cache.TryGetValue(key, out var dictionary);
      return dictionary;
    }

    private string GetCultureResourceCacheKey(string cultureName, string resourceName)
        => $"{currentTenant.Id?.ToString() ?? "host"}_{cultureName}_{resourceName}";
  }
}
