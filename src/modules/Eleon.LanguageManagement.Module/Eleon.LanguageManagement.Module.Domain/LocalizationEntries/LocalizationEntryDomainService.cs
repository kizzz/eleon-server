using Common.Module.Extensions;
using Logging.Module;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Localization;
using Volo.Abp.Uow;
using VPortal.LanguageManagement.Module.Entities;
using VPortal.LanguageManagement.Module.Repositories;

namespace VPortal.LanguageManagement.Module.LocalizationEntries
{

  public class LocalizationEntryDomainService : DomainService
  {
    private readonly IVportalLogger<LocalizationEntryDomainService> logger;
    private readonly ILocalizationEntryRepository localizationEntryRepository;
    private readonly LocalizationEntryCache cache;

    public LocalizationEntryDomainService(
        IVportalLogger<LocalizationEntryDomainService> logger,
        ILocalizationEntryRepository localizationEntryRepository,
        LocalizationEntryCache cache)
    {
      this.logger = logger;
      this.localizationEntryRepository = localizationEntryRepository;
      this.cache = cache;
    }

    public async Task SetLocalizationEntry(
        string cultureName,
        string resourceName,
        string key,
        string newValue)
    {
      try
      {
        var existingEntry = await localizationEntryRepository.FindByResourceAndKey(cultureName, resourceName, key);
        if (newValue.NonEmpty())
        {
          if (existingEntry == null)
          {
            var newEntry = new LocalizationEntryEntity(GuidGenerator.Create())
            {
              CultureName = cultureName,
              Key = key,
              Value = newValue,
              ResourceName = resourceName,
            };

            await localizationEntryRepository.InsertAsync(newEntry);
          }
          else
          {
            existingEntry.Value = newValue;
            await localizationEntryRepository.UpdateAsync(existingEntry);
          }
        }
        else if (existingEntry != null)
        {
          await localizationEntryRepository.DeleteAsync(existingEntry);
        }

        cache.ResetLocalizationDictionary(cultureName, resourceName);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public LocalizedString GetLocalizationEntry(string cultureName, string resourceName, string key)
    {
      LocalizedString result = null;
      try
      {
        var dict = cache.GetLocalizationDictionaryOrNull(cultureName, resourceName);
        result = dict?.GetOrNull(key);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    internal async Task FillLocalizationDictionary(string cultureName, string resourceName, Dictionary<string, LocalizedString> dict)
    {
      try
      {
        if (CurrentTenant.Id != null)
        {
          using (CurrentTenant.Change(null))
          {
            var hostDict = await GetLocalizationDictionaryAsync(cultureName, resourceName);
            hostDict.Fill(dict);
          }
        }

        var tenantDict = await GetLocalizationDictionaryAsync(cultureName, resourceName);
        tenantDict.Fill(dict);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task<StaticLocalizationDictionary> GetLocalizationDictionaryAsync(string cultureName, string resourceName)
        => await cache.GetOrCreateLocalizationDictionary(
                cultureName,
                resourceName,
                async () => await CreateLocalizationDictionary(cultureName, resourceName));

    private async Task<StaticLocalizationDictionary> CreateLocalizationDictionary(string cultureName, string resourceName)
    {
      var entries = await localizationEntryRepository.GetByResource(cultureName, resourceName);
      var dict = entries.ToDictionary(
          x => x.Key,
          x => new LocalizedString(x.Key, x.Value.NormalizeLineEndings()));

      return new StaticLocalizationDictionary(cultureName, dict);
    }

    public StaticLocalizationDictionary GetLocalizationDictionary(string cultureName, string resourceName)
    {
      try
      {
        var result = cache.GetLocalizationDictionaryOrNull(cultureName, resourceName);
        return result;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }
  }
}
