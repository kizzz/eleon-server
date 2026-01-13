using Common.Module.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace Authorization.Module.RequestLocalization
{
  public static class RequestLocalizationExtensions
  {
    public static void AddDefaultTenantRequestCultureProvider(this RequestLocalizationOptions options)
    {
      var provider = new DefaultTenantRequestCultureProvider();
      var insertAfterType = typeof(CookieRequestCultureProvider);
      options.RequestCultureProviders.InsertAfter(x => x.GetType() == insertAfterType, provider);

      var cultures = LanguageDefaults.GetDefaultSupportedCultures();
      options.AddSupportedCultures(cultures);
      options.AddSupportedUICultures(cultures);
    }
  }
}
