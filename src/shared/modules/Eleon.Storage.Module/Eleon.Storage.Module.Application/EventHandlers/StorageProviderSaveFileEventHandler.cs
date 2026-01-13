using Common.EventBus.Module;
using Eleon.AbpSdk.Lib.modules.Messages.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Blob.Module.VfsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Eleon.Storage.Module.Eleon.Storage.Module.Application.EventHandlers
{
  public class StorageProviderSaveFileEventHandler : IDistributedEventHandler<StorageProviderSaveFileMsg>, ITransientDependency
  {
    private readonly IVportalLogger<StorageProviderSaveFileEventHandler> _logger;
    private readonly VfsStorageProviderCacheManager _vfsProvidersCacheManager;
    private readonly IResponseContext _responseContext;
    public StorageProviderSaveFileEventHandler(IVportalLogger<StorageProviderSaveFileEventHandler> logger, VfsStorageProviderCacheManager vfsProvidersCacheManager, IResponseContext responseContext)
    {
      _logger = logger;
      _vfsProvidersCacheManager = vfsProvidersCacheManager;
      _responseContext = responseContext;
    }

    public async Task HandleEventAsync(StorageProviderSaveFileMsg eventData)
    {
      try
      {
        var provider = await _vfsProvidersCacheManager.ResolveProviderAsync(eventData.TenantId, eventData.StorageProviderId.ToString());

        if (provider != null)
        {
          using var ms = new MemoryStream(eventData.FileContent);
          var vfsSaveArgs = new VfsSaveArgs("./", eventData.FilePath, ms);
          await provider.SaveAsync(vfsSaveArgs);
        }
      }
      catch (Exception ex)
      {
        _logger.Capture(ex, "Error occurred while handling storage provider settings changed event");
      }
      finally
      {

      }
    }
  }
}
