using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace Eleoncore.SDK.RequestLocalization
{
  public static class EleoncoreSdkRequestLocalizationExtensions
  {
    public static void AddDefaultEleoncoreTenantRequestCultureProvider(this RequestLocalizationOptions options)
    {
      var provider = new EleoncoreSdkDefaultTenantRequestCultureProvider();
      var insertAfterType = typeof(CookieRequestCultureProvider);
      options.RequestCultureProviders.InsertAfter(x => x.GetType() == insertAfterType, provider);

      var cultures = LanguageDefaults.GetDefaultSupportedCultures();
      options.AddSupportedCultures(cultures);
      options.AddSupportedUICultures(cultures);
    }
  }
}
