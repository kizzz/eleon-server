using Lifecycle.Feature.Module.Application.Contracts.LifecycleManager;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.LifecycleFeatureModule.Lifecycle.Feature.Module.Application.Contracts.LifecycleManager;
using Volo.Abp;
using VPortal.Lifecycle.Feature.Module.LifecycleManager;

namespace VPortal.Lifecycle.Feature.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Lifecycle/Manager")]
  public class LifecycleManagerController : ModuleController, ILifecycleManagerAppService
  {
    private readonly ILifecycleManagerAppService lifecycleManagerAppService;
    private readonly IVportalLogger<LifecycleManagerController> _logger;

    public LifecycleManagerController(
        ILifecycleManagerAppService lifecycleManagerAppService,
        IVportalLogger<LifecycleManagerController> logger)
    {
      this.lifecycleManagerAppService = lifecycleManagerAppService;
      _logger = logger;
    }

    [HttpPost("Approve")]
    public async Task<bool> Approve(string documentObjectType, string documentId)
    {

      var response = await lifecycleManagerAppService.Approve(documentObjectType, documentId);


      return response;
    }

    [HttpGet("CanApprove")]
    public async Task<bool> CanApprove(string documentObjectType, string DocId)
    {

      var response = await lifecycleManagerAppService.CanApprove(documentObjectType, DocId);


      return response;
    }

    [HttpGet("GetTrace")]
    public async Task<StatesGroupAuditTreeDto> GetTrace(string documentObjectType, string DocId)
    {

      var response = await lifecycleManagerAppService.GetTrace(documentObjectType, DocId);


      return response;
    }

    [HttpPost("Reject")]
    public async Task<bool> Reject(string documentObjectType, string documentId, string reason)
    {

      var response = await lifecycleManagerAppService.Reject(documentObjectType, documentId, reason);


      return response;
    }

    [HttpGet("GetLifecycleStatus")]
    public async Task<LifecycleStatusDto> GetLifecycleStatus(string documentObjectType, string documentId)
    {

      var response = await lifecycleManagerAppService.GetLifecycleStatus(documentObjectType, documentId);


      return response;
    }

    [HttpGet("GetViewPermission")]
    public async Task<bool> GetViewPermissionAsync(Guid initiatorId, string documentObjectType, string documentId, bool review = true)
    {

      var response = await lifecycleManagerAppService.GetViewPermissionAsync(initiatorId, documentObjectType, documentId, review);


      return response;
    }

    [HttpGet("GetDocumentIdsByFilter")]
    public async Task<List<string>> GetDocumentIdsByFilterAsync(GetDocumentIdsByFilterRequestDto input)
    {

      var response = await lifecycleManagerAppService.GetDocumentIdsByFilterAsync(input);


      return response;
    }

    [HttpPost("StartExistingLifecycle")]
    public async Task<StatesGroupAuditTreeDto> StartExistingLifecycleAsync(string documentObjectType, string documentId)
    {

      var response = await lifecycleManagerAppService.StartExistingLifecycleAsync(documentObjectType, documentId);


      return response;
    }

    [HttpPost("StartNewLifecycle")]
    public async Task<StatesGroupAuditTreeDto> StartNewLifecycleAsync(StartNewLifecycleRequestDto input)
    {

      var response = await lifecycleManagerAppService.StartNewLifecycleAsync(input);


      return response;
    }

    [HttpGet("CanReview")]
    public async Task<bool> CanReview(string documentObjectType, string documentId)
    {

      var response = await lifecycleManagerAppService.CanReview(documentObjectType, documentId);

      return response;
    }
  }
}
