using EleonsoftSdk.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Web;
using TenantSettings.Module.Cache;
using Volo.Abp.MultiTenancy;

namespace Authorization.Module.TenantHostname
{
  public class HostnameTenantResolveContributor : TenantResolveContributorBase
  {
    public HostnameTenantResolveContributor()
    {
    }

    public override string Name => "Hostname";

    public override async Task ResolveAsync(ITenantResolveContext context)
    {
      var httpContext = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
      var hostnameCache = context.ServiceProvider.GetRequiredService<TenantSettingsCacheService>();

      try
      {
        await ResolveTenantFromHostname(context, httpContext, hostnameCache);
      }
      catch (Exception e)
      {
        context.ServiceProvider
            .GetRequiredService<ILogger<HostnameTenantResolveContributor>>()
            .LogError(e, "Error occured while resolving tenant");
      }
    }

    private async Task ResolveTenantFromHostname(
        ITenantResolveContext resolveContext,
        HttpContext httpContext,
        TenantSettingsCacheService settingsCache)
    {
      var hostname = EleonsoftTenantHelper.ExtractHostnameFromContext(httpContext);
      if (hostname.IsNullOrWhiteSpace())
      {
        return;
      }

      var (found, tenantId) = await settingsCache.GetTenantIdByUrl(hostname);
      if (found)
      {
        // if (found && baseUrlRight) proceed else throw Error
        resolveContext.Handled = true;
        resolveContext.TenantIdOrName = tenantId.ToString().IsNullOrEmpty() ? null : tenantId.ToString();
      }
    }
  }
}
