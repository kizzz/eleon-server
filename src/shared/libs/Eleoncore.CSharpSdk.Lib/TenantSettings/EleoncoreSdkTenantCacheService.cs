using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using Microsoft.Extensions.Caching.Memory;

namespace TenantSettings.Module.Cache
{
  public class EleoncoreSdkTenantCacheService
  {
    private readonly ITenantApi _tenantApi;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private const string CacheKey = "Tenants";

    public EleoncoreSdkTenantCacheService(
        ITenantApi tenantApi,
        IMemoryCache cache)
    {
      _tenantApi = tenantApi;
      _cache = cache;
    }

    public TenantManagementCommonTenantExtendedDto? GetTenant(Guid? tenantId)
    {
      var result = GetTenants().FirstOrDefault(t => t.Id == tenantId);

      if (result == null)
      {
        _cache.Remove(CacheKey);
        result = GetTenants().FirstOrDefault(t => t.Id == tenantId);
      }

      return result;
    }
    public List<TenantManagementCommonTenantExtendedDto> GetTenants()
    {
      if (!_cache.TryGetValue(CacheKey, out List<TenantManagementCommonTenantExtendedDto> cachedTenants) || cachedTenants == null || cachedTenants.Count == 0)
      {
        _tenantApi.UseApiAuth();
        var task = _tenantApi.CoreTenantGetCommonTenantExtendedListWithCurrentAsync();
        task.Wait();
        var response = task.Result;

        if (!response.IsOk)
        {
          throw new Exception("GetTenants sdk response was not OK");
        }

        cachedTenants = response.Ok();

        _cache.Set(CacheKey, cachedTenants, CacheDuration);
      }

      return cachedTenants;
    }
  }
}
