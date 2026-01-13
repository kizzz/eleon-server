using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace EleonS3.Application.EventHandlers;
public class TelemetryProviderIdChangedEventHandler : IDistributedEventHandler<TelemetryStorageProviderChangedMsg>
{
  private readonly ILogger<TelemetryProviderIdChangedEventHandler> _logger;
  private readonly VfsStorageProviderCacheManager _tenantTelemetrySettingsCache;
  public TelemetryProviderIdChangedEventHandler(ILogger<TelemetryProviderIdChangedEventHandler> logger, VfsStorageProviderCacheManager tenantTelemetrySettingsCache)
  {
    _logger = logger;
    _tenantTelemetrySettingsCache = tenantTelemetrySettingsCache;
  }

  public async Task HandleEventAsync(TelemetryStorageProviderChangedMsg eventData)
  {
    try
    {
      _tenantTelemetrySettingsCache.RefreshTenantTelemetryProvider(eventData.TenantId, eventData.StorageProviderId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while handling telemetry provider id changed event");
    }
  }
}
