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
  [Route("api/Lifecycle/Audits/StateActor")]
  public class StateActorAuditController : ModuleController, IStateActorAuditAppService
  {
    private readonly IStateActorAuditAppService stateActorAuditAppService;
    private readonly IVportalLogger<StateActorAuditController> _logger;

    public StateActorAuditController(
        IStateActorAuditAppService stateActorAuditAppService,
        IVportalLogger<StateActorAuditController> logger)
    {
      this.stateActorAuditAppService = stateActorAuditAppService;
      _logger = logger;
    }

    [HttpPost("Add")]
    public async Task<bool> Add(StateActorAuditDto stateActorAudit)
    {

      bool response = await stateActorAuditAppService.Add(stateActorAudit);


      return response;
    }

    [HttpDelete("Remove")]
    public async Task<bool> Remove(Guid id)
    {

      bool response = await stateActorAuditAppService.Remove(id);


      return response;
    }
  }
}
