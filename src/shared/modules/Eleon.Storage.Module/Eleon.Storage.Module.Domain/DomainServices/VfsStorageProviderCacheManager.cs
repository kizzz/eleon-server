
using Common.EventBus.Module;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Blob.Module;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Shared;
using System.Collections.Concurrent;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

namespace EleonsoftSdk.modules.StorageProvider.Module;
public class VfsStorageProviderCacheManager : IDisposable
{
  private readonly ILogger<VfsStorageProviderCacheManager> _logger;
  private readonly ConcurrentDictionary<string, string> _tenantTelemetrySettingsCache = new();
  private readonly IDistributedEventBus _eventBus;
  private readonly VfsBlobProviderCacheService _vfsBlobProviderCache;


  public VfsStorageProviderCacheManager(
      ILogger<VfsStorageProviderCacheManager> logger,
      IDistributedEventBus eventBus,
      VfsBlobProviderCacheService vfsBlobProviderCache)
  {
    _logger = logger;
    _eventBus = eventBus;
    _vfsBlobProviderCache = vfsBlobProviderCache;
  }


  public void RefreshTenantTelemetryProvider(Guid? tenantId, string newProviderId)
  {
    try
    {
      var tenantIdStr = tenantId?.ToString() ?? "nullTenant";
      if (!_tenantTelemetrySettingsCache.TryGetValue(tenantIdStr, out var existingProviderId) || existingProviderId == newProviderId)
      {
        return;
      }

      _vfsBlobProviderCache.Remove(BuildKey(tenantId, existingProviderId));

      _tenantTelemetrySettingsCache[tenantIdStr] = newProviderId;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while refreshing tenant telemetry provider");
      throw;
    }
  }

  public void UpdateBlobProviderCache(Guid? tenantId, string providerId)
  {
    _logger.LogDebug("Starting update of blob provider cache");
    try
    {
      _vfsBlobProviderCache.Remove(
          BuildKey(tenantId, providerId)
      );
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while updating blob provider cache");
      throw;
    }
    finally
    {
      _logger.LogDebug("Finished updating blob provider cache");
    }
  }
  public string GetCachedTelemetryProviderId(Guid? tenantId)
  {
    string result;

    var tenantIdStr = tenantId?.ToString() ?? "nullTenant";

    if (_tenantTelemetrySettingsCache.TryGetValue(tenantIdStr, out var cachedId))
    {
      result = cachedId;
    }
    else
    {
      result = BlobMessagingConsts.TelemetryStorageProviderKey;
    }

    return result;
  }

  public bool TryGetCachedProvider(Guid? tenantId, string? providerId, out IVfsBlobProvider? provider)
  {
    provider = null;
    if (providerId == null)
    {
      return false;
    }

    return _vfsBlobProviderCache.TryGet(BuildKey(tenantId, providerId), out provider);
  }

  public void CacheProviderMapping(Guid? tenantId, string providerId)
  {
    var tenantIdStr = tenantId?.ToString() ?? "nullTenant";

    _tenantTelemetrySettingsCache[tenantIdStr] = providerId; // overwrite-safe
  }

  public async Task<IVfsBlobProvider> ResolveProviderAsync(Guid? tenantId, string? realStorageProviderId)
  {
    // Try cached provider first
    if (TryGetCachedProvider(tenantId, realStorageProviderId, out var cachedProvider))
    {
      return cachedProvider!;
    }
    if (_vfsBlobProviderCache.TryGet("default", out var defaultProvider))
    {
      return defaultProvider!;
    }

    // Request provider info
    var request = new GetStorageProviderMsg
    {
      StorageProviderId = realStorageProviderId
    };

    var responseMsg = await _eventBus.RequestAsync<GetStorageProviderResponseMsg>(request);

    if (responseMsg?.StorageProvider == null)
    {
      throw new Exception($"Storage provider for tenant '{tenantId}' not found.");
    }

    if (realStorageProviderId == BlobMessagingConsts.TelemetryStorageProviderKey)
    {
      // Cache telemetry provider id
      CacheProviderMapping(tenantId, responseMsg.StorageProvider.Id.ToString());
    }

    // Create and cache provider
    var result = VfsBlobProvidersFactory.Create(responseMsg.StorageProvider, tenantId);
    _vfsBlobProviderCache.Register(
        BuildKey(tenantId, responseMsg.StorageProvider.Id.ToString()),
        result
    );

    return result;
  }


  private string BuildKey(Guid? tenantId, string storageProviderId)
  {
    return $"{tenantId};{storageProviderId}";
  }

  public void Dispose()
  {
    // VfsBlobProviderCacheService is a singleton and will be disposed by DI container
    // No need to dispose here
  }
}
