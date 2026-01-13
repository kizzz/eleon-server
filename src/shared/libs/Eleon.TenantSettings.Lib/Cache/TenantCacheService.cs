using Common.EventBus.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Options;
using SharedModule.modules.MultiTenancy.Module;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace TenantSettings.Module.Cache
{
  public class TenantCacheService : ISingletonDependency
  {
    private readonly IDistributedEventBus _eventBus;
    private readonly ICurrentTenant currentTenant;
    private readonly IOptions<EleonMultiTenancyOptions> _options;

    // Cache to store tenant data
    private List<TenantEto> _tenantsCache = new List<TenantEto>();

    // Lock object for thread safety
    private readonly object _cacheLock = new object();

    public TenantCacheService(
        IDistributedEventBus eventBus,
        ICurrentTenant currentTenant,
        IOptions<EleonMultiTenancyOptions> options)
    {
      _eventBus = eventBus;
      this.currentTenant = currentTenant;
      _options = options;
    }

    /// <summary>
    /// Get all tenants from cache. If the cache is empty, it will be loaded.
    /// </summary>
    /// <returns>List of tenants</returns>
    public List<TenantEto> GetTenants()
    {
      if (_options.Value.Enabled == false)
      {
        return [];
      }

      // If the cache is empty, reload it
      if (!_tenantsCache.Any())
      {
        ReloadCacheAsync().Wait();
      }

      // Return the cached tenants
      return _tenantsCache;
    }

    public async Task<List<TenantEto>> GetTenantsAsync()
    {
      if (_options.Value.Enabled == false)
      {
        return [];
      }

      // If the cache is empty, reload it
      if (!_tenantsCache.Any())
      {
        await ReloadCacheAsync();
      }

      // Return the cached tenants
      return _tenantsCache;
    }

    /// <summary>
    /// Reloads the cache from the database.
    /// </summary>
    public async Task ReloadCacheAsync()
    {
      using (currentTenant.Change(null))
      {
        // Lock to ensure thread safety
        lock (_cacheLock)
        {
          // Clear existing cache
          _tenantsCache.Clear();
        }

        // Load tenants from repository
        var tenants = await _eventBus.RequestAsync<AllTenantsGotMsg>(new GetAllTenantsMsg());

        // Lock again to update cache safely
        lock (_cacheLock)
        {
          _tenantsCache = tenants.Tenants.ToList();
        }
      }
    }
  }
}
