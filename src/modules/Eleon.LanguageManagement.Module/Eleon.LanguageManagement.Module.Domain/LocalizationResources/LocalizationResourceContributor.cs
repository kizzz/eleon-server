using Common.Module.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Localization;
using VPortal.LanguageManagement.Module.Languages;
using VPortal.LanguageManagement.Module.LocalizationEntries;

namespace VPortal.LanguageManagement.Module.LocalizationResources
{
  public class LocalizationResourceContributor : ILocalizationResourceContributor
  {
    private string resourceName;
    private IServiceProvider serviceProvider;

    public bool IsDynamic => true;

    public void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
      // No sync actions needed.
    }

    public async Task FillAsync(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
      if (cultureName == LanguageDefaults.DevelopmentCulture)
      {
        await serviceProvider.GetRequiredService<DevLanguageLocalizationProvider>().FillDevelopmentLanguageDict(resourceName, dictionary);
      }
      else
      {
        await serviceProvider.GetRequiredService<LocalizationEntryDomainService>().FillLocalizationDictionary(cultureName, resourceName, dictionary);
      }
    }

    public LocalizedString GetOrNull(string cultureName, string name)
    {
      return serviceProvider.GetRequiredService<LocalizationEntryDomainService>().GetLocalizationEntry(cultureName, resourceName, name);
    }

    public async Task<IEnumerable<string>> GetSupportedCulturesAsync()
    {
      var enabledLangs = await serviceProvider.GetRequiredService<LanguageDomainService>().GetEnabledLanguages();
      return enabledLangs
          .Select(x => x.CultureName)
          .Concat([LanguageDefaults.DevelopmentCulture])
          .ToList();
    }

    public void Initialize(LocalizationResourceInitializationContext context)
    {
      resourceName = context.Resource.ResourceName;
      serviceProvider = context.ServiceProvider; // store, don't resolve here
    }
  }
}
