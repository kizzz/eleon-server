//using Logging.Module;
//using Microsoft.AspNetCore.Mvc;
//using ModuleCollector.Storage.Module.Storage.Module.Application.Contracts.Blob;
//using Volo.Abp;
//using VPortal.Storage.Module;


//namespace ModuleCollector.Storage.Module.Storage.Module.HttpApi.Controllers;
//[Area(ModuleRemoteServiceConsts.ModuleName)]
//[RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
//[Route("api/Storage/Blob")]
//public class BlobController : ModuleController, IBlobAppService
//{
//    private readonly IVportalLogger<BlobController> logger;
//    private readonly IBlobAppService blobAppService;

//    public BlobController(
//        IVportalLogger<BlobController> logger,
//        IBlobAppService blobAppService

//        )
//    {
//        this.logger = logger;
//        this.blobAppService = blobAppService;
//    }

//    [HttpGet("Get")]
//    public async Task<string> GetAsync(string settingsGroup, string blobName)
//    {

//        var response = await blobAppService.GetAsync(settingsGroup, blobName);


//        return response;
//    }

//    [HttpPost("Remove")]
//    public async Task<bool> RemoveAsync(string settingsGroup, string blobName)
//    {

//        var response = await blobAppService.RemoveAsync(settingsGroup, blobName);


//        return response;
//    }

//    [HttpPost("Save")]
//    public async Task<bool> SaveAsync(SaveBlobRequestDto request)
//    {

//        var response = await blobAppService.SaveAsync(request);


//        return response;
//    }
//}
