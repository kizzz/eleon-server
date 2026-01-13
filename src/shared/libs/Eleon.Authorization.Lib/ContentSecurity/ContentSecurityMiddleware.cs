using Microsoft.AspNetCore.Http;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Authorization.Module.ContentSecurity
{
  public class ContentSecurityMiddleware : IMiddleware, ITransientDependency
  {
    private const string ContentSecurityPolicyHeaderName = "Content-Security-Policy";
    private readonly TenantSettingsCacheService tenantSettingsCache;
    private readonly ICurrentTenant currentTenant;

    public ContentSecurityMiddleware(TenantSettingsCacheService tenantSettingsCache, ICurrentTenant currentTenant)
    {
      this.tenantSettingsCache = tenantSettingsCache;
      this.currentTenant = currentTenant;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      var settings = await tenantSettingsCache.GetTenantSettings(currentTenant.Id);
      var allowedHosts = settings.TenantContentSecurityHosts;
      string formattedHosts = allowedHosts.Select(x => x.EnsureEndsWith('/')).JoinAsString(" ");
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
