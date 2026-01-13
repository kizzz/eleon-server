using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Lifecycle.Feature.Module.Audits;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;

namespace VPortal.Lifecycle.Feature.Module.Controllers.Audits
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Lifecycle/Audits/State")]
  public class StateAuditController : ModuleController, IStateAuditAppService
  {
    private readonly IStateAuditAppService stateAuditAppService;
    private readonly IVportalLogger<StateAuditController> _logger;

    public StateAuditController(
        IStateAuditAppService stateAuditAppService,
        IVportalLogger<StateAuditController> logger)
    {
      this.stateAuditAppService = stateAuditAppService;
      _logger = logger;
    }

    [HttpPost("Add")]
    public async Task<bool> Add(StateAuditDto stateAudit)
    {

      bool response = await stateAuditAppService.Add(stateAudit);


      return response;
    }

    [HttpDelete("Remove")]
    public async Task<bool> Remove(Guid id)
    {

      bool response = await stateAuditAppService.Remove(id);


      return response;
    }
  }
}
