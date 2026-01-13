using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;
using VPortal.Lifecycle.Feature.Module.Templates;

namespace VPortal.Lifecycle.Feature.Module.Controllers.Templates
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Lifecycle/Templates/StateActor")]
  public class StateActorTemplateController : ModuleController, IStateActorTemplateAppService
  {
    private readonly IStateActorTemplateAppService stateActorAuditAppService;
    private readonly IVportalLogger<StateActorTemplateController> _logger;

    public StateActorTemplateController(
        IStateActorTemplateAppService stateActorAuditAppService,
        IVportalLogger<StateActorTemplateController> logger)
    {
      this.stateActorAuditAppService = stateActorAuditAppService;
      _logger = logger;
    }

    [HttpPost("Add")]
    public async Task<bool> Add(StateActorTemplateDto stateActorTemplate)
    {

      bool response = await stateActorAuditAppService.Add(stateActorTemplate);


      return response;
    }

    [HttpPut("Enable")]
    public async Task<bool> Enable(StateSwitchDto stateSwitchDto)
    {

      var response = await stateActorAuditAppService.Enable(stateSwitchDto);


      return response;
    }

    [HttpGet("GetAll")]
    public async Task<List<StateActorTemplateDto>> GetAllAsync(Guid stateId)
    {

      var response = await stateActorAuditAppService.GetAllAsync(stateId);


      return response;
    }

    [HttpDelete("Remove")]
    public async Task<bool> Remove(Guid id)
    {

      bool response = await stateActorAuditAppService.Remove(id);


      return response;
    }

    [HttpPut("Update")]
    public async Task<bool> Update(StateActorTemplateDto stateActorTemplate)
    {

      bool response = await stateActorAuditAppService.Update(stateActorTemplate);


      return response;
    }

    [HttpPut("UpdateOrderIndexes")]
    public async Task<bool> UpdateOrderIndexes(Dictionary<Guid, int> order)
    {

      bool response = await stateActorAuditAppService.UpdateOrderIndexes(order);


      return response;
    }
  }
}
