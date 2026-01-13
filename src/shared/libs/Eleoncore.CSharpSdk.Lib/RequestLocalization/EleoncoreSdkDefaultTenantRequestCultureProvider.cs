using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using EleonsoftProxy.Api;

namespace Eleoncore.SDK.RequestLocalization
{
  internal class EleoncoreSdkDefaultTenantRequestCultureProvider : IRequestCultureProvider
  {
    public async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
      var languageService = httpContext.RequestServices.GetRequiredService<ITenantLocalizationApi>();
      var response = await languageService.LanguageManagementTenantLocalizationGetTenantLanguageAsync();
      if (response.TryOk(out var defaultLang))
      {
        return new ProviderCultureResult(defaultLang.CultureName, defaultLang.UiCultureName);
      }
      return new ProviderCultureResult("en", "en");
    }
  }
}
