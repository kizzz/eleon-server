using Common.Module.Constants;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Data;
using System.Threading.Tasks;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Entities.Conditions;
using VPortal.Lifecycle.Feature.Module.Permissions;

namespace VPortal.Lifecycle.Feature.Module.Conditions
{
  [Authorize(LifecyclePermissions.LifecycleManager)]
  public class ConditionAppService : ModuleAppService, IConditionAppService
  {
    private readonly ConditionDomainService conditionDomainService;
    private readonly IVportalLogger<ConditionAppService> logger;

    public ConditionAppService(
        ConditionDomainService conditionDomainService,
        IVportalLogger<ConditionAppService> logger
        )
    {
      this.conditionDomainService = conditionDomainService;
      this.logger = logger;
    }
    public async Task<bool> AddCondition(ConditionDto conditionDto)
    {

      bool result = false;
      try
      {
        var entity = ObjectMapper.Map<ConditionDto, ConditionEntity>(conditionDto);
        result = await conditionDomainService.AddCondition(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }
    public async Task<bool> UpdateCondition(ConditionDto conditionDto)
    {

      bool result = false;
      try
      {
        var entity = ObjectMapper.Map<ConditionDto, ConditionEntity>(conditionDto);
        result = await conditionDomainService.UpdateCondition(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<bool> AddRule(Guid conditionId, RuleDto ruleDto)
    {
      bool result = false;
      try
      {
        var entity = ObjectMapper.Map<RuleDto, RuleEntity>(ruleDto);
        result = await conditionDomainService.AddRule(conditionId, entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
    public async Task<bool> UpdateRule(Guid conditionId, RuleDto ruleDto)
    {
      bool result = false;
      try
      {
        var entity = ObjectMapper.Map<RuleDto, RuleEntity>(ruleDto);
        result = await conditionDomainService.UpdateRule(conditionId, entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<ConditionDto> GetCondition(LifecycleConditionTargetType lifecycleConditionType, Guid refId)
    {

      ConditionDto result = null;
      try
      {
        var entity = await conditionDomainService.GetCondition(lifecycleConditionType, refId);
        result = ObjectMapper.Map<ConditionEntity, ConditionDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<bool> RemoveCondition(Guid conditionId)
    {

      bool result = false;
      try
      {
        result = await conditionDomainService.RemoveCondition(conditionId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;
    }

    public async Task<bool> RemoveRule(Guid conditionId, Guid ruleId)
    {

      bool result = false;
      try
      {
        result = await conditionDomainService.RemoveRule(conditionId, ruleId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task ReplyCheckRule(ReplyCheckRuleDto input)
    {
      try
      {
        await conditionDomainService.ReplyCheckRule(input.Id, input.Result);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }
  }
}
