using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Lifecycle.Feature.Module.Conditions;

namespace VPortal.Lifecycle.Feature.Module.Controllers.Conditions
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Lifecycle/Conditions")]
  public class ConditionController : ModuleController, IConditionAppService
  {
    private readonly IConditionAppService appService;
    private readonly IVportalLogger<ConditionController> logger;

    public ConditionController(
        IConditionAppService stateActorAuditAppService,
        IVportalLogger<ConditionController> logger)
    {
      this.appService = stateActorAuditAppService;
      this.logger = logger;
    }

    [HttpPost("AddCondition")]
    public async Task<bool> AddCondition(ConditionDto conditionDto)
    {


      var response = await appService.AddCondition(conditionDto);


      return response;

    }

    [HttpPost("AddRule")]
    public async Task<bool> AddRule(Guid conditionId, RuleDto ruleDto)
    {

      var response = await appService.AddRule(conditionId, ruleDto);


      return response;
    }

    [HttpGet("GetCondition")]
    public async Task<ConditionDto> GetCondition(LifecycleConditionTargetType lifecycleConditionType, Guid refId)
    {

      var response = await appService.GetCondition(lifecycleConditionType, refId);


      return response;
    }

    [HttpDelete("RemoveCondition")]
    public async Task<bool> RemoveCondition(Guid conditionId)
    {

      var response = await appService.RemoveCondition(conditionId);


      return response;
    }
    [HttpDelete("DeleteRule")]
    public async Task<bool> RemoveRule(Guid conditionId, Guid ruleId)
    {

      var response = await appService.RemoveRule(conditionId, ruleId);


      return response;
    }

    [HttpPost("ReplyCheckRule")]
    public async Task ReplyCheckRule(ReplyCheckRuleDto input)
    {

      await appService.ReplyCheckRule(input);

    }

    [HttpPut("UpdateCondition")]
    public async Task<bool> UpdateCondition(ConditionDto conditionDto)
    {

      var response = await appService.UpdateCondition(conditionDto);


      return response;
    }

    [HttpPut("UpdateRule")]
    public async Task<bool> UpdateRule(Guid conditionId, RuleDto ruleDto)
    {

      var response = await appService.UpdateRule(conditionId, ruleDto);


      return response;
    }
  }
}
