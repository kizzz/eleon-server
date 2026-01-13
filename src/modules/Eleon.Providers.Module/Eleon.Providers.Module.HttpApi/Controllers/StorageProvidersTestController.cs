using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using SharedModule.modules.Blob.Module.Models;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Storage.Module.StorageProviders;

namespace VPortal.Storage.Module.Controllers;

[Area(ProvidersRemoteServiceConsts.ModuleName)]
[RemoteService(Name = ProvidersRemoteServiceConsts.RemoteServiceName)]
[Route("api/Storage/StorageProvidersTest")]
public class StorageProvidersTestController : ModuleController, IStorageProvidersTestAppService
{
  private readonly IStorageProvidersTestAppService appService;
  private readonly IVportalLogger<StorageProvidersTestController> _logger;

  public StorageProvidersTestController(
      IStorageProvidersTestAppService appService,
      IVportalLogger<StorageProvidersTestController> logger)
  {
    this.appService = appService;
    _logger = logger;
  }

  [HttpPost("TestStorageProvider")]
  public async Task<bool> TestStorageProvider(StorageProviderDto provider)
  {

    var response = await appService.TestStorageProvider(provider);


    return response;
  }
}
