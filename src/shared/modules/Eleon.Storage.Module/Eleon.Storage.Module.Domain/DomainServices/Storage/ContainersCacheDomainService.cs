using Logging.Module;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using VPortal.Storage.Module.Cache;

namespace VPortal.Storage.Module.DomainServices
{
  public class ContainersCacheDomainService : DomainService, ISingletonDependency
  {
    private readonly IVportalLogger<ContainersCacheDomainService> logger;

    public ContainersCacheDomainService(
        IVportalLogger<ContainersCacheDomainService> logger)
    {
      this.logger = logger;
    }

    private readonly ConcurrentDictionary<string, IBlobContainer> cache = new();

    public IBlobContainer GetOrAddCacheEntry(ContainerInTenantCacheKey key, Func<IBlobContainer> containerFactory)
    {
      IBlobContainer result = null;
      try
      {
        result = cache.GetOrAdd(key.ToString(), _ => containerFactory());
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public bool RemoveCacheEntry(ContainerInTenantCacheKey key)
    {
      bool result = false;
      try
      {
        result = cache.Remove(key.ToString(), out _);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    /// <summary>
    /// Invalidates all cache entries matching the tenant and provider key (any settings version).
    /// Called when storage provider settings change.
    /// </summary>
    public int InvalidateCacheEntries(Guid? tenantId, string providerKey)
    {
      int removedCount = 0;
      try
      {
        var prefix = $"{tenantId};{providerKey};";
        var keysToRemove = new List<string>();

        foreach (var key in cache.Keys)
        {
          if (key.StartsWith(prefix, StringComparison.Ordinal))
          {
            keysToRemove.Add(key);
          }
        }

        foreach (var key in keysToRemove)
        {
          if (cache.TryRemove(key, out var container))
          {
            removedCount++;
            // Dispose container if it implements IDisposable
            if (container is IDisposable disposable)
            {
              try { disposable.Dispose(); } catch { }
            }
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return removedCount;
    }
  }
}
