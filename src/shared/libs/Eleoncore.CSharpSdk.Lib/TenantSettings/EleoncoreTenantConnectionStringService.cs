using EleoncoreProxy.Api;
using EleoncoreProxy.Model;
using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;

namespace Eleoncore.SDK.TenantSettings
{
  public class EleoncoreTenantConnectionStringService : ISingletonDependency
  {
    private readonly EleoncoreSdkTenantCacheService eleoncoreSdkTenantCacheService;
    private readonly IApplicationConnectionStringApi _applicationConnectionStringApi;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

    public EleoncoreTenantConnectionStringService(
        EleoncoreSdkTenantCacheService eleoncoreSdkTenantCacheService,
        IApplicationConnectionStringApi applicationConnectionStringApi,
        IMemoryCache memoryCache)
    {
      this.eleoncoreSdkTenantCacheService = eleoncoreSdkTenantCacheService;
      _applicationConnectionStringApi = applicationConnectionStringApi;
      _memoryCache = memoryCache;
    }

    public async Task<string> GetConnectionStringOrDefault(Guid? tenantId, string applicationName, string name, string defaultConnectionString)
    {
      string cacheKey = GetCacheKey(tenantId, applicationName, name);

      if (_memoryCache.TryGetValue(cacheKey, out string cachedConnectionString))
      {
        return cachedConnectionString;
      }

      var tenant = eleoncoreSdkTenantCacheService.GetTenant(tenantId);


      var connectionStrings = new List<SitesManagementConnectionStringDto>();
      if (tenant != null)
      {
        _applicationConnectionStringApi.UseApiAuth();
        var response = await _applicationConnectionStringApi.SitesManagementApplicationConnectionStringGetConnectionStringsAsync(tenantId ?? Guid.Empty);
        var connectionStringsResponse = response.Ok();
        if (connectionStringsResponse != null)
        {
          connectionStrings = connectionStringsResponse;
        }
      }

      var connectionString = connectionStrings.FirstOrDefault(c =>
      {
        return c.ApplicationName == applicationName
                  // && c.Name == name
                  ;
      });

      string result = connectionString?.ConnectionString ?? defaultConnectionString;

      _memoryCache.Set(cacheKey, result, _cacheExpiration);

      return result;
    }

    private string GetCacheKey(Guid? tenantId, string applicationName, string name)
    {
      return $"Tenant:{tenantId}:Application:{applicationName}:Name:{name}";
    }
  }
}
