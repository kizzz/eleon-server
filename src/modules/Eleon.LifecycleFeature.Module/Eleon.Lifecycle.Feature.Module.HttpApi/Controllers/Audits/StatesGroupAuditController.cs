using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Lifecycle.Feature.Module.Audits;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;

namespace VPortal.Lifecycle.Feature.Module.Controllers.Audits
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Lifecycle/Audits/StatesGroup")]
  public class StatesGroupAuditController : ModuleController, IStatesGroupAuditAppService
  {
    private readonly IStatesGroupAuditAppService statesGroupAuditsAppService;
    private readonly IVportalLogger<StatesGroupAuditController> logger;

    public StatesGroupAuditController(
        IStatesGroupAuditAppService statesGroupAuditsAppService,
        IVportalLogger<StatesGroupAuditController> logger)
    {
      this.statesGroupAuditsAppService = statesGroupAuditsAppService;
      this.logger = logger;
    }

    [HttpPost("Add")]
    public async Task<bool> Add(StatesGroupAuditDto statesGroupAudit)
    {

      bool response = await statesGroupAuditsAppService.Add(statesGroupAudit);


      return response;
    }

    [HttpDelete("Remove")]
    public async Task<bool> Remove(Guid id)
    {

      bool response = await statesGroupAuditsAppService.Remove(id);


      return response;
    }

    [HttpDelete("DeepCancel")]
    public async Task<bool> DeepCancel(string docType, string documentId)
    {

      bool response = await statesGroupAuditsAppService.DeepCancel(docType, documentId);


      return response;
    }

    [HttpGet("GetReports")]
    public async Task<PagedResultDto<StatesGroupAuditReportDto>> GetReports(PendingApprovalRequestDto input)
    {

      var response = await statesGroupAuditsAppService.GetReports(input);


      return response;
    }

    [HttpGet("Get")]
    public async Task<StatesGroupAuditDto> GetById(Guid id)
    {

      var response = await statesGroupAuditsAppService.GetById(id);
      return response;
    }
  }
}
