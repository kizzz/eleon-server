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
  [Route("api/Lifecycle/Templates/State")]
  public class StateTemplateController : ModuleController, IStateTemplateAppService
  {
    private readonly IStateTemplateAppService stateTemplateAppService;
    private readonly IVportalLogger<StateTemplateController> logger;

    public StateTemplateController(
        IStateTemplateAppService stateTemplateAppService,
        IVportalLogger<StateTemplateController> logger)
    {
      this.stateTemplateAppService = stateTemplateAppService;
      this.logger = logger;
    }

    [HttpPost("Add")]
    public async Task<bool> Add(StateTemplateDto stateTemplate)
    {

      bool response = await stateTemplateAppService.Add(stateTemplate);


      return response;
    }

    [HttpPut("Enable")]
    public async Task<bool> Enable(StateSwitchDto input)
    {

      var response = await stateTemplateAppService.Enable(input);


      return response;
    }

    [HttpGet("GetAll")]
    public async Task<List<StateTemplateDto>> GetAllAsync(Guid groupId)
    {

      var response = await stateTemplateAppService.GetAllAsync(groupId);


      return response;
    }

    [HttpDelete("Remove")]
    public async Task<bool> Remove(Guid groupId, Guid stateId)
    {

      bool response = await stateTemplateAppService.Remove(groupId, stateId);


      return response;
    }

    [HttpPut("UpdateApprovalType")]
    public async Task<bool> UpdateApprovalType(UpdateApprovalTypeDto update)
    {

      bool response = await stateTemplateAppService.UpdateApprovalType(update);


      return response;
    }

    [HttpPut("UpdateName")]
    public async Task<bool> UpdateName(Guid id, string name)
    {

      var response = await stateTemplateAppService.UpdateName(id, name);


      return response;
    }

    [HttpPut("UpdateOrderIndex")]
    public async Task<bool> UpdateOrderIndex(UpdateOrderIndexDto update)
    {

      bool response = await stateTemplateAppService.UpdateOrderIndex(update);


      return response;
    }

    [HttpPut("UpdateOrderIndexes")]
    public async Task<bool> UpdateOrderIndexes(Dictionary<Guid, int> order)
    {

      bool response = await stateTemplateAppService.UpdateOrderIndexes(order);


      return response;
    }
  }
}
