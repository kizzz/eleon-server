using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using TenantSettings.Module.Cache;
using Volo.Abp.Http;
using Volo.Abp.MultiTenancy;

namespace Eleoncore.Module.TenantHostname
{
  public class EleonsoftTenantHostnameCorsPolicyProvider : ICorsPolicyProvider
  {
    private readonly TenantSettingsCacheService tenantSettingsCache;
    private readonly IEnumerable<EleonsoftCorsPolicyConfigurator> configurators;
    private readonly IMemoryCache cache;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10); // Cache duration

    public EleonsoftTenantHostnameCorsPolicyProvider(
        TenantSettingsCacheService tenantSettingsCache,
        IEnumerable<EleonsoftCorsPolicyConfigurator> configurators,
        IMemoryCache cache)
    {
      this.tenantSettingsCache = tenantSettingsCache;
      this.configurators = configurators;
      this.cache = cache;
    }

    public async Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
    {
      // Determine the hostname from HttpContext
      var hostname = context.Request.Host.Host;

      // Use hostname as the cache key
      var cacheKey = $"CorsPolicy_{hostname}";

      var currentTenantId = context.RequestServices.GetRequiredService<ICurrentTenant>().Id;

      if (!cache.TryGetValue(cacheKey, out CorsPolicy cachedPolicy))
      {
        // Cache miss - fetch and build the policy
        var urlsResponse = await tenantSettingsCache.GetTenantSettings(currentTenantId);
        var urls = urlsResponse.TenantHostnames;

        var builder = new CorsPolicyBuilder(urls.ToArray())
            .WithExposedHeaders(AbpHttpConsts.AbpErrorFormat)
            .WithExposedHeaders(AbpHttpConsts.AbpTenantResolveError)
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();

        foreach (var cfg in configurators)
        {
          cfg.ConfigurePolicies(builder);
        }

        cachedPolicy = builder.Build();

        // Store the policy in the cache with the hostname as the key
        cache.Set(cacheKey, cachedPolicy, CacheExpiration);
      }

      return cachedPolicy;
    }
  }
}
