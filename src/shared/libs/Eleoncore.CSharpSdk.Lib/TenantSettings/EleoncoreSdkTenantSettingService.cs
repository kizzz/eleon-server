using Eleoncore.Module.TenantHostname;
using Eleoncore.SDK.Helpers;
using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SharedModule.modules.MultiTenancy.Module;
using System;
using System.Collections.Concurrent;
using System.Web;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http;

namespace TenantSettings.Module.Cache
{

  public class EleoncoreSdkTenantSettingService : ICorsPolicyProvider, ISingletonDependency
  {
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24); // Cache duration

    private readonly IEnumerable<EleoncoreCorsPolicyConfigurator> _configurators;
    private readonly EleonMultiTenancyOptions _options;
    private readonly ITenantSettingsCacheApi _tenantSettingsCacheApi;
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<Guid?, SemaphoreSlim> _tenantSettingsSemaphores = new();

    public EleoncoreSdkTenantSettingService(
        ITenantSettingsCacheApi tenantSettingsCacheApi,
        IMemoryCache cache,
        IEnumerable<EleoncoreCorsPolicyConfigurator> configurators,
        IOptions<EleonMultiTenancyOptions> options)
    {
      _tenantSettingsCacheApi = tenantSettingsCacheApi;
      _cache = cache;
      _configurators = configurators;
      _options = options.Value;
    }

    public async ValueTask<TenantManagementTenantSettingsCacheValueDto?> GetTenantSettingsAsync(Guid? tenantId, CancellationToken cancellationToken = default)
    {
      var cacheKey = $"TenantSettings_{tenantId ?? Guid.Empty}";

      var semaphore = _tenantSettingsSemaphores.GetOrAdd(tenantId, _ => new SemaphoreSlim(1, 1));

      await semaphore.WaitAsync(cancellationToken);
      try
      {
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
          entry.AbsoluteExpirationRelativeToNow = CacheExpiration;

          var response = await _tenantSettingsCacheApi.CoreTenantSettingsCacheGetTenantSettingsAsync(tenantId ?? Guid.Empty, cancellationToken);
          return response?.Ok();
        });
      }
      finally
      {
        semaphore.Release();
      }
    }

    public async Task<(bool handled, Guid? tenantId)> ResolveTenantFromHostname(HttpContext httpContext)
    {
      var hostname = ExtractHostnameFromContext(httpContext);
      if (string.IsNullOrWhiteSpace(hostname))
      {
        throw new Exception("Tenant hostname was not found");
      }
      var tenant = _options.TenantDomains.FirstOrDefault(kv => kv.Value.Contains(hostname));

      if (!tenant.Value.IsNullOrEmpty())
      {
        AddNewAuthorityIfNotExists(hostname);
        return Guid.TryParse(tenant.Key, out var tId) ? (true, tId) : (true, null);
      }

      // Use hostname as the cache key
      var cacheKey = $"Tenant_{hostname}";

      if (!_cache.TryGetValue(cacheKey, out (bool handled, Guid? tenantId) cachedResult))
      {
        // Cache miss - fetch tenant from API
        var response = await _tenantSettingsCacheApi.CoreTenantSettingsCacheGetTenantByUrlAsync(hostname);

        if (!response.IsOk)
        {
          throw new Exception($"Tenant was not resolved with hostname {hostname} and API Key");
        }

        var result = response.Ok();

        if (result.IsFound.Value)
        {
          var tenantId = result.TenantId;
          cachedResult = (true, tenantId);

          // Cache the result
          _cache.Set(cacheKey, cachedResult, CacheExpiration);
          AddNewAuthorityIfNotExists(hostname);
        }
        else
        {
          // Cache tenant not found to avoid repeated lookups
          cachedResult = (false, null);
          _cache.Set(cacheKey, cachedResult, CacheExpiration);

          throw new Exception($"Tenant not found by hostname {hostname}");
        }
      }

      return cachedResult;
    }

    public void AddNewAuthorityIfNotExists(string authority)
    {
      var authorities = _cache.GetOrCreate<List<string>>("Authorities", (_) => new List<string>() { });

      if (!authorities.Contains(authority))
      {
        authorities.Add(authority);
        _cache.Set("Authorities", authorities, CacheExpiration);

      }
      EleoncoreIssuerValidatorHelper.Authorities = authorities;
    }

    private string ExtractHostnameFromContext(HttpContext httpContext)
    {
      if (httpContext.Request.Headers.TryGetValue("X-Forwarded-Host", out var forwardedHosts))
      {
        return $"https://{forwardedHosts.FirstOrDefault()}";
      }
      if (httpContext.Request.Headers.TryGetValue("host", out var hosts))
      {
        return $"https://{hosts.FirstOrDefault()}";
      }

      if (httpContext.Request.Query.TryGetValue("ReturnUrl", out var redirectUrls))
      {
        var decodedUrl = HttpUtility.UrlDecode(redirectUrls.FirstOrDefault());
        var redirectUrlQuery = HttpUtility.ParseQueryString(decodedUrl);
        var innerRedirectUris = redirectUrlQuery.GetValues("redirect_uri");
        if (innerRedirectUris?.Any() == true)
        {
          return HttpUtility.UrlDecode(innerRedirectUris.First());
        }
      }

      if (httpContext.Request.Query.TryGetValue("redirect_uri", out var redirectUris))
      {
        return redirectUris.FirstOrDefault();
      }

      if (httpContext.Request.Headers.TryGetValue("Referer", out var referers))
      {
        string referer = referers.First();
        if (referer.EndsWith('/'))
        {
          referer = referer[..^1];
        }

        return referer;
      }

      if (httpContext.Request.Headers.TryGetValue("Origin", out var origins))
      {
        return origins.FirstOrDefault();
      }

      return null;
    }

    public async ValueTask<string> GetBaseTenantUrl(HttpContext context, Guid? tenantId, bool? security = null)
    {
      string host = context.Request.Host.Value;
      if (security == null)
      {
        return Https(host);
      }

      Guid settingTenantId = tenantId == null ? Guid.Empty : tenantId.Value;
      var settings = await GetTenantSettingsAsync(settingTenantId);

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
      Guid settingTenantId = tenantId == null ? Guid.Empty : tenantId.Value;
      var settings = await GetTenantSettingsAsync(settingTenantId);
      return settings?.TenantSecureHostnames.Contains(host) ?? false;
    }

    private static string Https(string host) => $"https://{host}";

    public async Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
    {
      // Determine the hostname from HttpContext
      var hostname = context.Request.Host.Host;

      // Use hostname as the cache key
      var cacheKey = $"CorsPolicy_{hostname}";

      if (!_cache.TryGetValue(cacheKey, out CorsPolicy cachedPolicy))
      {
        // Cache miss - fetch and build the policy
        var urlsResponse = await _tenantSettingsCacheApi.CoreTenantSettingsCacheGetApplicationUrlsAsync();
        var urls = urlsResponse.Ok() ?? [];

        var builder = new CorsPolicyBuilder(urls.ToArray())
            .WithExposedHeaders(AbpHttpConsts.AbpErrorFormat)
            .WithExposedHeaders(AbpHttpConsts.AbpTenantResolveError)
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();

        foreach (var cfg in _configurators)
        {
          cfg.ConfigurePolicies(builder);
        }

        cachedPolicy = builder.Build();

        // Store the policy in the cache with the hostname as the key
        _cache.Set(cacheKey, cachedPolicy, CacheExpiration);
      }

      return cachedPolicy;
    }
  }
}
