using Common.EventBus.Module;
using Common.Module.Extensions;
using Common.Module.Keys;
using Logging.Module;
using Messaging.Module.Messages;
using Storage.Module.DomainServices;
using Storage.Module.LightweightStorage;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Storage.Module.LightweightStorageItems
{
  public class LightweightStorageItemEventService :
      IDistributedEventHandler<SaveLightweightStorageItemMsg>,
      ITransientDependency
  {
    private readonly IVportalLogger<LightweightStorageItemEventService> logger;
    private readonly IResponseContext responseContext;
    private readonly LightweightStorageDomainService lightweightStorageDomainService;

    public LightweightStorageItemEventService(
        IVportalLogger<LightweightStorageItemEventService> logger,
        IResponseContext responseContext,
        LightweightStorageDomainService lightweightStorageDomainService)
    {
      this.logger = logger;
      this.responseContext = responseContext;
      this.lightweightStorageDomainService = lightweightStorageDomainService;
    }

    public async Task HandleEventAsync(SaveLightweightStorageItemMsg eventData)
    {
      var msg = eventData;
      var response = new ActionCompletedMsg();
      try
      {
        var defaults = new LightweightStorageOptions();
        var options = new LightweightStorageOptions()
        {
          MaxSize = msg.MaxSize ?? defaults.MaxSize,
          CacheExpiration = msg.CacheExpiration ?? defaults.CacheExpiration,
          MaxSizeUnit = msg.MaxSizeUnit ?? defaults.MaxSizeUnit,
          RequiredPermissions = msg.RequiredPermissions ?? defaults.RequiredPermissions,
        };

        var key = LightweightStorageKey.Parse(msg.Key);

        if (!string.IsNullOrEmpty(msg.BlobBase64))
        {
          await lightweightStorageDomainService.SaveLightweightItem(key.SettingsGroup, key.BlobName, msg.BlobBase64, options);
        }
        else
        {
          await lightweightStorageDomainService.Delete(key.SettingsGroup, key.BlobName);
        }

        response.Success = true;
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
        response.Error = ex.Message;
      }
      finally
      {
        await responseContext.RespondAsync(response);
      }

    }
  }
}
