using Common.Module.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Lifecycle.Feature.Module.Conditions
{
  public interface IConditionAppService : IApplicationService
  {
    public Task<ConditionDto> GetCondition(LifecycleConditionTargetType lifecycleConditionType, Guid refId);
    public Task<bool> AddCondition(ConditionDto conditionDto);
    public Task<bool> AddRule(Guid conditionId, RuleDto ruleDto);
    public Task<bool> RemoveRule(Guid conditionId, Guid ruleId);
    public Task<bool> UpdateRule(Guid conditionId, RuleDto ruleDto);
    public Task<bool> UpdateCondition(ConditionDto conditionDto);
    public Task<bool> RemoveCondition(Guid conditionId);
    Task ReplyCheckRule(ReplyCheckRuleDto input);
  }
}
