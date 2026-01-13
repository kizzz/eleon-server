using Logging.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Localization.External;

namespace VPortal.LanguageManagement.Module.LocalizationResources
{
  public record LocalizedResourceString(string Resource, LocalizedString String);

  public class LocalizationResourceProvider : ITransientDependency
  {
    private readonly AbpLocalizationOptions localizationOptions;
    private readonly IVportalLogger<LocalizationResourceProvider> logger;
    private readonly IStringLocalizerFactory stringLocalizerFactory;
    private readonly ITransientCachedServiceProvider transientCachedServiceProvider;

    public LocalizationResourceProvider(
        IVportalLogger<LocalizationResourceProvider> logger,
        IOptions<AbpLocalizationOptions> options,
        IStringLocalizerFactory stringLocalizerFactory,
        ITransientCachedServiceProvider transientCachedServiceProvider)
    {
      localizationOptions = options.Value;
      this.logger = logger;
      this.stringLocalizerFactory = stringLocalizerFactory;
      this.transientCachedServiceProvider = transientCachedServiceProvider;
    }

    public async Task<List<string>> GetLocalizationResources()
            => localizationOptions
                    .Resources
                    .Values
                    .Select(x => x.ResourceName)
                    .Union(
                        await transientCachedServiceProvider
                            .GetRequiredService<IExternalLocalizationStore>()
                            .GetResourceNamesAsync())
                    .Where(x => !IsAbpResource(x))
                    .ToList();

    public async Task<List<LocalizedResourceString>> GetLocalizationStrings(string culture, List<string> localizationResources)
    {
      var result = new List<LocalizedResourceString>();
      try
      {
        var oldUICulture = CultureInfo.CurrentUICulture;
        var oldCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentUICulture = new CultureInfo(culture);
        CultureInfo.CurrentCulture = new CultureInfo(culture);

        var strings = await GetCurrentCultureLocalizationStrings(localizationResources);

        CultureInfo.CurrentUICulture = oldUICulture;
        CultureInfo.CurrentCulture = oldCulture;
        return strings;
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }

      return result;
    }

    private async Task<List<LocalizedResourceString>> GetCurrentCultureLocalizationStrings(List<string> localizationResources)
    {
      var stringsByResource = new List<LocalizedResourceString>();
      foreach (var resourceName in localizationResources)
      {
        var localizer = await stringLocalizerFactory
            .CreateByResourceNameOrNullAsync(resourceName);
        if (localizer != null)
        {
          var resourceStrings = await localizer.GetAllStringsAsync(false, false, true);
          var localizedStrings = resourceStrings
              .Select(s => new LocalizedResourceString(resourceName, s))
              .ToList();

          stringsByResource.AddRange(localizedStrings);
        }
      }

      return stringsByResource;
    }

    private static bool IsAbpResource(string resource)
        => resource.StartsWith("Abp") || resource.StartsWith("CmsKit");
  }
}
