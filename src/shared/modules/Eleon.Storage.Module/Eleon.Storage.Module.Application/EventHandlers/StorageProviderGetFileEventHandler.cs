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
  public class StorageProviderGetFileEventHandler : IDistributedEventHandler<StorageProviderGetFileMsg>, ITransientDependency
  {
    private readonly IVportalLogger<StorageProviderGetFileEventHandler> _logger;
    private readonly VfsStorageProviderCacheManager _vfsProvidersCacheManager;
    private readonly IResponseContext _responseContext;
    public StorageProviderGetFileEventHandler(IVportalLogger<StorageProviderGetFileEventHandler> logger, VfsStorageProviderCacheManager vfsProvidersCacheManager, IResponseContext responseContext)
    {
      _logger = logger;
      _vfsProvidersCacheManager = vfsProvidersCacheManager;
      _responseContext = responseContext;
    }

    public async Task HandleEventAsync(StorageProviderGetFileMsg eventData)
    {
      var response = new StorageProviderGetFileResponseMsg();
      response.FileContent = [];
      try
      {
        var provider = await _vfsProvidersCacheManager.ResolveProviderAsync(eventData.TenantId, eventData.StorageProviderId.ToString());

        if (provider != null)
        {
          var vfsGetArgs = new VfsGetArgs("./", eventData.FilePath);
          var fileStream = await provider.GetAllBytesOrNullAsync(vfsGetArgs) ?? [];
          response.FileContent = fileStream;
        }
      }
      catch (Exception ex)
      {
        _logger.Capture(ex, "Error occurred while handling storage provider settings changed event");
      }
      finally
      {
        await _responseContext.RespondAsync(response);


      }
    }
  }
}
