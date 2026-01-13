using Common.EventBus.Module;
using EleonsoftSdk.Helpers;
using EleonsoftSdk.Messages.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using TenantSettings.Module.Cache;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace Eleoncore.SDK.RequestLocalization
{
  public class EleonsoftDefaultTenantRequestCultureProvider : IRequestCultureProvider
  {
    public async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
      var languageService = httpContext.RequestServices.GetRequiredService<IDistributedEventBus>();
      var tenantId = httpContext.RequestServices.GetRequiredService<ICurrentTenant>().Id;
      var response = await languageService.RequestAsync<GetTenantLocalizationSettingsResponseMsg>(new GetTenantLocalizationSettingsRequestMsg { TenantId = tenantId });
      if (response != null)
      {
        return new ProviderCultureResult(response.CultureName, response.UiCultureName);
      }
      return new ProviderCultureResult("en", "en");
    }
  }
}
