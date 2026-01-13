using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace TenantSettings.Module.EventHandlers
{
  public class TenantSettingsUpdatedEventService :
      IDistributedEventHandler<TenantSettingsUpdatedMsg>,
      ITransientDependency
  {
    private readonly ILogger<TenantSettingsUpdatedEventService> logger;
    private readonly TenantSettingsCacheService cache;

    public TenantSettingsUpdatedEventService(
        ILogger<TenantSettingsUpdatedEventService> logger,
        TenantSettingsCacheService cache)
    {
      this.logger = logger;
      this.cache = cache;
    }

    public async Task HandleEventAsync(TenantSettingsUpdatedMsg eventData)
    {
      try
      {
        await cache.UpdateCache();
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Error occurred while handling tenant settings updated event");
      }
    }
  }
}
