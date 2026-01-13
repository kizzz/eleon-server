using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Storage.Module.LightweightStorageItems;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Storage.Module;
using VPortal.Storage.Remote.Application.Contracts;

namespace Storage.Module.Controllers
{
  [Area(StorageRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = StorageRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Storage/LightweightStorageItems")]
  public class LightweightStorageItemController : StorageModuleController, ILightweightStorageItemAppService
  {
    private readonly IVportalLogger<LightweightStorageItemController> logger;
    private readonly ILightweightStorageItemAppService appService;

    public LightweightStorageItemController(
        IVportalLogger<LightweightStorageItemController> logger,
        ILightweightStorageItemAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetLightweightItem")]
    public async Task<string> GetLightweightItem(string key)
    {
      var response = await appService.GetLightweightItem(key);
      return response;
    }

    [HttpGet("GetLightweightItems")]
    public async Task<List<string>> GetLightweightItems(List<string> keys)
    {
      var response = await appService.GetLightweightItems(keys);
      return response;
    }
  }
}
