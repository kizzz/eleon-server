using Common.Module.Constants;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.LanguageManagement.Module.LocalizationResources
{
  public class DevLanguageLocalizationProvider : ISingletonDependency
  {
    private readonly LocalizationResourceProvider localizationResourceProvider;
    private readonly AsyncLocal<bool> isGettingDevDict = new();

    public DevLanguageLocalizationProvider(LocalizationResourceProvider localizationResourceProvider)
    {
      this.localizationResourceProvider = localizationResourceProvider;
    }

    public async Task FillDevelopmentLanguageDict(string resource, Dictionary<string, LocalizedString> dict)
    {
      if (isGettingDevDict.Value)
      {
        return;
      }

      try
      {
        isGettingDevDict.Value = true;
        var strings = await localizationResourceProvider.GetLocalizationStrings(LanguageDefaults.DefaultCulture, [resource]);
        foreach (var str in strings)
        {
          string key = str.String.Name;
          string value = $"{resource}::{key}";
          dict[key] = new(key, value);
        }
      }
      finally
      {
        isGettingDevDict.Value = false;
      }
    }
  }
}
