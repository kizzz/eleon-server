using Authorization.Module.RequestLocalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace VPortal.LanguageManagement.Module.Providers
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
