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
  public class StorageProviderDeleteFilesEventHandler : IDistributedEventHandler<StorageProviderDeleteFilesMsg>, ITransientDependency
  {
    private readonly IVportalLogger<StorageProviderDeleteFilesEventHandler> _logger;
    private readonly VfsStorageProviderCacheManager _vfsProvidersCacheManager;
    public StorageProviderDeleteFilesEventHandler(IVportalLogger<StorageProviderDeleteFilesEventHandler> logger, VfsStorageProviderCacheManager vfsProvidersCacheManager)
    {
      _logger = logger;
      _vfsProvidersCacheManager = vfsProvidersCacheManager;
    }

    public async Task HandleEventAsync(StorageProviderDeleteFilesMsg eventData)
    {
      try
      {
        var provider = await _vfsProvidersCacheManager.ResolveProviderAsync(eventData.TenantId, eventData.StorageProviderId.ToString());
        var vfsDeleteArgs = new VfsDeleteArgs("./", string.Empty);
        foreach (var key in eventData.Keys)
        {
          vfsDeleteArgs.ContainerName = key;

          await provider.DeleteAsync(vfsDeleteArgs);
        }

      }
      catch (Exception ex)
      {
        _logger.Capture(ex, "Error occurred while handling storage provider delete files event");
      }
      finally
      {
      }
    }
  }
}
