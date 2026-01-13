using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using VPortal.Storage.Remote.Application.Contracts;
using VPortal.Storage.Remote.Application.Contracts.Storage;

namespace VPortal.Storage.Remote.HttpApi.Storage
{
  [Area(ProvidersRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ProvidersRemoteServiceConsts.RemoteServiceName)]
  [Route("api/storage/remote/")]
  public class StorageRemoteController : StorageRemoteModuleController, IStorageRemoteAppService
  {
    private readonly IStorageRemoteAppService storageRemoteAppService;
    private readonly IVportalLogger<StorageRemoteController> logger;

    public StorageRemoteController(IStorageRemoteAppService storageRemoteAppService, IVportalLogger<StorageRemoteController> logger)
    {
      this.storageRemoteAppService = storageRemoteAppService;
      this.logger = logger;
    }

    [HttpDelete]
    [Route("Delete")]
    public async Task<bool> Delete(string settingsGroup, string blobName)
    {

      var response = await storageRemoteAppService.Delete(settingsGroup, blobName);


      return response;
    }

    [HttpGet]
    [Route("Exists")]
    public async Task<bool> Exists(string settingsGroup, string blobName)
    {

      var response = await storageRemoteAppService.Exists(settingsGroup, blobName);


      return response;
    }

    [HttpGet]
    [Route("GetBase64")]
    public async Task<string> GetBase64(string settingsGroup, string blobName)
    {

      var response = await storageRemoteAppService.GetBase64(settingsGroup, blobName);


      return response;
    }

    [HttpPost]
    [Route("Save")]
    public async Task<bool> Save(SaveBase64Request request)
    {

      var response = await storageRemoteAppService.Save(request);


      return response;
    }
  }
}
