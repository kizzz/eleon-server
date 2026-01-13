using Common.Module.Extensions;
using Logging.Module;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Localization;
using Volo.Abp.Uow;
using VPortal.LanguageManagement.Module.Languages;
using VPortal.LanguageManagement.Module.LocalizationEntries;
using VPortal.LanguageManagement.Module.LocalizationResources;

namespace VPortal.LanguageManagement.Module.LocalizationOverrides
{

  public class LocalizationOverrideDomainService : DomainService
  {
    private readonly LocalizationEntryDomainService localizationEntryDomainService;
    private readonly LanguageDomainService languageDomainService;
    private readonly ILanguageProvider languageProvider;
    private readonly LocalizationResourceProvider localizationResourceProvider;
    private readonly IStringLocalizerFactory stringLocalizerFactory;
    private readonly IVportalLogger<LocalizationOverrideDomainService> logger;

    public LocalizationOverrideDomainService(
        LocalizationEntryDomainService localizationEntryDomainService,
        LanguageDomainService languageDomainService,
        ILanguageProvider languageProvider,
        LocalizationResourceProvider localizationResourceProvider,
        IStringLocalizerFactory stringLocalizerFactory,
        IVportalLogger<LocalizationOverrideDomainService> logger)
    {
      this.localizationEntryDomainService = localizationEntryDomainService;
      this.languageDomainService = languageDomainService;
      this.languageProvider = languageProvider;
      this.localizationResourceProvider = localizationResourceProvider;
      this.stringLocalizerFactory = stringLocalizerFactory;
      this.logger = logger;
    }

    public async Task<LocalizationInformation> GetLocalizationInformation()
    {
      var result = new LocalizationInformation();
      try
      {
        result.Languages = [.. await languageProvider.GetLanguagesAsync()];
        result.LocalizationResources = await localizationResourceProvider.GetLocalizationResources();

        var defaultLang = await languageDomainService.GetDefaultLanguage();
        result.DefaultCulture = defaultLang.CultureName;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<KeyValuePair<int, List<OverriddenLocalizationString>>> GetLocalizationStrings(
        string baseCulture,
        string targetCulture,
        List<string> localizationResources,
        string searchQuery,
        int skip,
        int take,
        string sorting,
        bool emptyTargetsOnly)
    {
      KeyValuePair<int, List<OverriddenLocalizationString>> result = default;
      try
      {
        var targetStrings = await localizationResourceProvider.GetLocalizationStrings(targetCulture, localizationResources);
        var baseStrings = await localizationResourceProvider.GetLocalizationStrings(baseCulture, localizationResources);

        var overridenStrings = CreateOverriddenStrings(targetCulture, baseStrings, targetStrings);

        var paginatedOverridenStrings = PaginateLocalizationStrings(overridenStrings, searchQuery, skip, take, sorting, emptyTargetsOnly);

        result = KeyValuePair.Create(paginatedOverridenStrings.Key, paginatedOverridenStrings.Value);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private List<OverriddenLocalizationString> CreateOverriddenStrings(
        string targetCulture,
        List<LocalizedResourceString> baseStrings,
        List<LocalizedResourceString> targetStrings)
    {
      // Pre-load all localization dictionaries
      var resourceNames = baseStrings.Select(x => x.Resource).Distinct().ToList();
      var localizationDictionaries = new Dictionary<string, StaticLocalizationDictionary>();

      foreach (var resourceName in resourceNames)
      {
        var dict = localizationEntryDomainService.GetLocalizationDictionary(targetCulture, resourceName);
        if (dict != null)
        {
          localizationDictionaries[resourceName] = dict;
        }
      }

      // Create lookup dictionary for O(1) target string access
      var targetStringLookup = targetStrings
          .GroupBy(x => (x.Resource, x.String.Name))
          .ToDictionary(
              g => g.Key,
              g => g.First());

      var result = new List<OverriddenLocalizationString>(baseStrings.Count);

      foreach (var baseString in baseStrings)
      {
        if (!targetStringLookup.TryGetValue((baseString.Resource, baseString.String.Name), out var targetString))
        {
          var nullTarget = new LocalizedString(baseString.String.Name, string.Empty);
          result.Add(new OverriddenLocalizationString(
              baseString.String,
              nullTarget,
              baseString.Resource,
              false));
        }
        else
        {
          bool isOverride = false;
          if (localizationDictionaries.TryGetValue(targetString.Resource, out var dict))
          {
            var overrideEntry = dict.GetOrNull(targetString.String.Name);
            isOverride = overrideEntry != null && overrideEntry.Value == targetString.String.Value;
          }

          result.Add(new OverriddenLocalizationString(
              baseString.String,
              targetString.String,
              targetString.Resource,
              isOverride));
        }
      }

      return result;
    }

    private KeyValuePair<int, List<OverriddenLocalizationString>> PaginateLocalizationStrings(
        List<OverriddenLocalizationString> resourceStrings,
        string searchQuery,
        int skip,
        int take,
        string sorting,
        bool emptyTargetsOnly)
    {
      var filtered = resourceStrings
          .AsQueryable()
          .WhereIf(searchQuery.NonEmpty(), str =>
              str.FullKey.Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase)
              || str.Base.Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase)
              || str.Target.Contains(searchQuery, StringComparison.InvariantCultureIgnoreCase))
          .WhereIf(emptyTargetsOnly, str => str.Target.IsNullOrWhiteSpace())
          .OrderBy(sorting)
          .ToList();

      var paged = filtered
          .Skip(skip)
          .Take(take)
          .ToList();

      return KeyValuePair.Create(filtered.Count, paged);
    }
  }
}
