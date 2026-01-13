using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Module.RequestLocalization
{
  internal class DefaultTenantRequestCultureProvider : IRequestCultureProvider
  {
    public async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
      var languageService = httpContext.RequestServices.GetRequiredService<RequestLanguageProvider>();
      var defaultLang = await languageService.GetTenantLanguage();
      return new ProviderCultureResult(defaultLang.CultureName, defaultLang.UiCultureName);
    }
  }
}
