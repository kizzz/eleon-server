using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Auditor.Module.AuditHistoryRecords;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Auditor.Module.Controllers
{
  [Area(AuditorRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = AuditorRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Auditor/History")]
  public class AuditHistoryRecordController : ModuleController, IAuditHistoryRecordAppService
  {
    private readonly IAuditHistoryRecordAppService appService;
    private readonly IVportalLogger<IAuditHistoryRecordAppService> _logger;

    public AuditHistoryRecordController(
        IAuditHistoryRecordAppService appService,
        IVportalLogger<IAuditHistoryRecordAppService> logger)
    {
      this.appService = appService;
      _logger = logger;
    }

    [HttpGet("GetModuleFieldSets")]
    public async Task<PagedResultDto<DocumentVersionEntity>> GetDocumentHistory(DocumentHistoryRequest request)
    {

      var response = await appService.GetDocumentHistory(request);


      return response;
    }
  }
}
