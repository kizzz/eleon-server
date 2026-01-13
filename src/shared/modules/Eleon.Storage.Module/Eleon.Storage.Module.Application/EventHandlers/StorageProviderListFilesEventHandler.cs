using Common.EventBus.Module;
using Eleon.AbpSdk.Lib.modules.Messages.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Blob.Module.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Eleon.Storage.Module.Eleon.Storage.Module.Application.EventHandlers
{
  public class StorageProviderListFilesEventHandler : IDistributedEventHandler<StorageProviderListFilesMsg>, ITransientDependency
  {
    private readonly IVportalLogger<StorageProviderListFilesEventHandler> _logger;
    private readonly IResponseContext _responseContext;
    private readonly VfsStorageProviderCacheManager _vfsProvidersCacheManager;
    public StorageProviderListFilesEventHandler(IVportalLogger<StorageProviderListFilesEventHandler> logger, VfsStorageProviderCacheManager vfsProvidersCacheManager, IResponseContext responseContext)
    {
      _logger = logger;
      _vfsProvidersCacheManager = vfsProvidersCacheManager;
      _responseContext = responseContext;
    }

    public async Task HandleEventAsync(StorageProviderListFilesMsg eventData)
    {
      var response = new StorageProviderListFilesResponseMsg();
      try
      {
        var provider = await _vfsProvidersCacheManager.ResolveProviderAsync(eventData.TenantId, eventData.StorageProviderId.ToString());

        var listResult = await provider.ListAsync(new VfsListArgs("./", "./"));

        response.Files = listResult.ToList();
      }
      catch (Exception ex)
      {

        _logger.Capture(ex, "Error occurred while handling storage provider list files request");
      }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }
  }
}
