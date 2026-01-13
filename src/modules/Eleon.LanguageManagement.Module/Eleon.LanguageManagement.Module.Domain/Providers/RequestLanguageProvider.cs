using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using VPortal.LanguageManagement.Module.Languages;

namespace Authorization.Module.RequestLocalization
{
  public class RequestLanguageProvider : ISingletonDependency
  {
    private readonly Dictionary<string, LanguageCacheEntry> languageCache = new();
    private readonly IVportalLogger<RequestLanguageProvider> logger;
    private readonly LanguageDomainService languageDomainService;
    private readonly ICurrentTenant currentTenant;
    private readonly ICancellationTokenProvider _cancellationTokenProvider;
    private readonly SemaphoreSlim cacheSemaphore = new(1);

    public RequestLanguageProvider(
        IVportalLogger<RequestLanguageProvider> logger,
        LanguageDomainService languageDomainService,
        ICurrentTenant currentTenant,
        ICancellationTokenProvider cancellationTokenProvider)
    {
      this.logger = logger;
      this.languageDomainService = languageDomainService;
      this.currentTenant = currentTenant;
      _cancellationTokenProvider = cancellationTokenProvider;
    }

    public async Task<LanguageCacheEntry> GetTenantLanguage()
    {
      LanguageCacheEntry result = null;
      try
      {
        string key = GetCacheKey();
        if (languageCache.TryGetValue(key, out var cacheValue))
        {
          result = cacheValue;
        }
        else
        {
          await cacheSemaphore.WaitAsync(_cancellationTokenProvider.Token);
          try
          {
            result = await RetreiveTenantLanguage();
            languageCache[key] = result;
          }
          finally
          {
            cacheSemaphore.Release();
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SetTenantLanguage(string cultureName, string uiCultureName)
    {
      try
      {
        await cacheSemaphore.WaitAsync(_cancellationTokenProvider.Token);
        try
        {
          languageCache[GetCacheKey()] = new LanguageCacheEntry(cultureName, uiCultureName);
        }
        finally
        {
          cacheSemaphore.Release();
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task<LanguageCacheEntry> RetreiveTenantLanguage()
    {
      var languageEntry = await languageDomainService.GetDefaultLanguage();
      return new LanguageCacheEntry(languageEntry.CultureName, languageEntry.UiCultureName);
    }

    private string GetCacheKey() => currentTenant.Id?.ToString() ?? "host";
  }

  public record LanguageCacheEntry(string CultureName, string UiCultureName);
}
