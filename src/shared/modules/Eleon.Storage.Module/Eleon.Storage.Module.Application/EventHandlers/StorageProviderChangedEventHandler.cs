using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.Storage.Module.DomainServices;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace EleonS3.Domain;
public class StorageProviderChangedEventHandler : IDistributedEventHandler<StorageProviderSettingsChangedMsg>
{
  private readonly ILogger<StorageProviderChangedEventHandler> _logger;
  private readonly VfsStorageProviderCacheManager _tenantTelemetrySettingsCache;
  private readonly ContainersCacheDomainService _containersCache;

  public StorageProviderChangedEventHandler(
      ILogger<StorageProviderChangedEventHandler> logger,
      VfsStorageProviderCacheManager tenantTelemetrySettingsCache,
      ContainersCacheDomainService containersCache)
  {
    _logger = logger;
    _tenantTelemetrySettingsCache = tenantTelemetrySettingsCache;
    _containersCache = containersCache;
  }

  public async Task HandleEventAsync(StorageProviderSettingsChangedMsg eventData)
  {
    try
    {
      _tenantTelemetrySettingsCache.UpdateBlobProviderCache(eventData.TenantId, eventData.StorageProviderId);
      
      // Invalidate container cache entries for this provider (all settings versions)
      var providerId = eventData.StorageProviderId?.ToString() ?? string.Empty;
      var removedCount = _containersCache.InvalidateCacheEntries(eventData.TenantId, providerId);
      if (removedCount > 0)
      {
        _logger.LogDebug("Invalidated {Count} container cache entries for tenant {TenantId}, provider {ProviderId}",
            removedCount, eventData.TenantId, providerId);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while handling storage provider settings changed event");
    }
  }
}
