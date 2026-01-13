using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace Eleoncore.SDK.RequestLocalization
{
  public static class EleonsoftSdkRequestLocalizationExtensions
  {
    public static void AddDefaultTenantRequestCultureProvider(this RequestLocalizationOptions options)
    {
      var provider = new EleonsoftDefaultTenantRequestCultureProvider();
      var insertAfterType = typeof(CookieRequestCultureProvider);
      options.RequestCultureProviders.InsertAfter(x => x.GetType() == insertAfterType, provider);

      var cultures = LanguageDefaults.GetDefaultSupportedCultures();
      options.AddSupportedCultures(cultures);
      options.AddSupportedUICultures(cultures);
    }
  }
}
