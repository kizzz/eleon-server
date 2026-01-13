using Microsoft.AspNetCore.Http;
using TenantSettings.Module.Cache;
using Volo.Abp.MultiTenancy;

namespace Eleoncore.Module.ContentSecurity
{
  public class EleonsoftContentSecurityMiddleware : IMiddleware
  {
    private const string ContentSecurityPolicyHeaderName = "Content-Security-Policy";
    private readonly TenantSettingsCacheService _tenantSettingsCache;
    private readonly ICurrentTenant currentTenant;

    public EleonsoftContentSecurityMiddleware(TenantSettingsCacheService tenantSettings, ICurrentTenant currentTenant)
    {
      this._tenantSettingsCache = tenantSettings;
      this.currentTenant = currentTenant;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      if (!currentTenant.IsAvailable)
      {
        await next(context);
        return;
      }
      Guid tenantId = currentTenant.Id == null ? Guid.Empty : currentTenant.Id.Value;
      var settings = await _tenantSettingsCache.GetTenantSettings(currentTenant.Id);

      var allowedHosts = settings.TenantSetting.ContentSecurityHosts;
      string formattedHosts = allowedHosts.Select(x => x.Hostname.EnsureEndsWith('/')).JoinAsString(" ");
      if (formattedHosts.Trim().Length == 0)
      {
        formattedHosts = "none";
      }

      string policy = string.Join(" ", "frame-ancestors", formattedHosts) + ";";

      var response = context.Response;
      response.Headers[ContentSecurityPolicyHeaderName] = "frame-ancestors *;";

      await next(context);
    }
  }
}
