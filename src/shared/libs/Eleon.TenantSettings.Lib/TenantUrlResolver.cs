using Microsoft.AspNetCore.Http;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;

namespace Authorization.Module.TenantHostname
{
  public class TenantUrlResolver : ITransientDependency
  {
    private const string SecureMarker = "secure";
    private readonly TenantSettingsCacheService tenantSettingsCache;

    public TenantUrlResolver(TenantSettingsCacheService tenantSettingsCache)
    {
      this.tenantSettingsCache = tenantSettingsCache;
    }

    public async ValueTask<string> GetBaseTenantUrl(HttpContext context, Guid? tenantId, bool? security = null)
    {
      string host = context.Request.Host.Value;
      if (security == null)
      {
        return Https(host);
      }

      var settings = await tenantSettingsCache.GetTenantSettings(tenantId);
      if (security == true)
      {
        if (settings.TenantSecureHostnames.Contains(host))
        {
          return Https(host);
        }

        return Https(settings.TenantSecureHostnames.First());
      }

      if (settings.TenantNonSecureHostnames.Contains(host))
      {
        return Https(host);
      }

      return Https(settings.TenantNonSecureHostnames.First());
    }

    public async ValueTask<bool> IsSecureHost(HttpContext context, Guid? tenantId)
    {
      string host = context.Request.Host.Value;
      var settings = await tenantSettingsCache.GetTenantSettings(tenantId);
      return settings.TenantSecureHostnames.Contains(host);
    }

    private static string Https(string host) => $"https://{host}";
  }
}
