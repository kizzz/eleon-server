using System;
using System.Data;
using Common.EventBus.Module;
using Common.Module.Constants;
using Logging.Module;
using Messaging.Module.Messages;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using VPortal.Lifecycle.Feature.Module.Entities.Conditions;
using VPortal.Lifecycle.Feature.Module.Repositories.Conditions;

namespace VPortal.Lifecycle.Feature.Module.DomainServices
{
  public class ConditionDomainService : DomainService
  {
    private readonly IVportalLogger<ConditionDomainService> logger;
    private readonly IDistributedEventBus _eventBus;
    private readonly IConditionRepository repository;

    public ConditionDomainService(
      IVportalLogger<ConditionDomainService> logger,
      IDistributedEventBus eventBus,
      IConditionRepository repository
    )
    {
      this.logger = logger;
      this._eventBus = eventBus;
      this.repository = repository;
    }

    public async Task<bool> CheckShouldSkip(
      string documentObjectType,
      string documentId,
      LifecycleConditionTargetType lifecycleConditionTargetType,
      Guid refId
    )
    {
      bool result = false;
      try
      {
        var condition = await GetCondition(lifecycleConditionTargetType, refId);

        if (condition == null)
        {
          return result;
        }

        var response = await _eventBus.RequestAsync<AuditCurrentVersionGotMsg>(
          new GetAuditCurrentVersionMsg
          {
            RefDocumentObjectType = documentObjectType,
            RefDocumentId = documentId,
          }
        );
        var currentVersion = response.CurrentVersion;
        if (currentVersion != null)
        {
          var docResponse = await _eventBus.RequestAsync<AuditDocumentGotMsg>(
            new GetAuditDocumentMsg
            {
              AuditedDocumentId = documentId,
              AuditedDocumentObjectType = documentObjectType,
              Version = currentVersion.Version,
            }
          );
          result = await CheckShouldSkip(condition, docResponse.AuditedDocument.Data);
        }
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

    public async Task<bool> CheckShouldSkip(ConditionEntity condition, string sourceXml)
    {
      bool result = false;
      try
      {
        foreach (var rule in condition.Rules)
        {
          var checkedRule = await CheckRule(rule, sourceXml);
          result = checkedRule;
          if (condition.ConditionType == LifecycleConditionType.Or && result)
          {
            return true;
          }
          else if (condition.ConditionType == LifecycleConditionType.And && !result)
          {
            return false;
          }
        }

        if (condition.ConditionResultType == LifecycleConditionResultType.ActivateOnSuccess)
        {
          result = !result;
        }
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

    public async Task<bool> CheckRule(RuleEntity rule, string sourceXml)
    {
      bool result = false;
      try
      {
        await _eventBus.PublishAsync(
          new LifecycleRuleCheckMsg()
          {
            SourceXml = sourceXml,
            Function = rule.Function,
            FunctionType = rule.FunctionType,
          }
        );

        // TODO:
        // save as waiting for response and
        // wait for result through sdk in ReplyCheckRule method
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

    public async Task ReplyCheckRule(string id, bool result)
    {
      try
      {
        // TODO:
        // handle check rule reply
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task<bool> AddCondition(ConditionEntity createEntity)
    {
      bool result = false;
      try
      {
        var entity = new ConditionEntity(GuidGenerator.Create())
        {
          //Rules = createEntity.Rules.Select(r => new RuleEntity(GuidGenerator.Create())
          //{
          //    Function = r.Function,
          //    FunctionType = r.FunctionType,
          //}).ToList(),
          RefId = createEntity.RefId,
          ConditionType = createEntity.ConditionType,
          ConditionTargetType = createEntity.ConditionTargetType,
          ConditionResultType = createEntity.ConditionResultType,
          IsEnabled = true,
        };

        await repository.InsertAsync(entity, true);

        result = true;
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

    public async Task<bool> UpdateCondition(ConditionEntity createEntity)
    {
      bool result = false;
      try
      {
        var condition = await repository.GetAsync(createEntity.Id, true);
        var entity = new ConditionEntity(GuidGenerator.Create())
        {
          Rules = (createEntity.Rules ?? new List<RuleEntity>())
            .Select(r => new RuleEntity(GuidGenerator.Create())
            {
              Function = r.Function,
              FunctionType = r.FunctionType,
            })
            .ToList(),
          RefId = createEntity.RefId,
          ConditionTargetType = createEntity.ConditionTargetType,
          ConditionType = createEntity.ConditionType,
          ConditionResultType = createEntity.ConditionResultType,
          IsEnabled = true,
        };

        await repository.InsertAsync(entity, true);

        result = true;
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

    public async Task<bool> AddRule(Guid conditionId, RuleEntity entity)
    {
      bool result = false;
      try
      {
        var condition = await repository.GetAsync(conditionId, true);

        condition.Rules.Add(
          new RuleEntity(GuidGenerator.Create())
          {
            Function = entity.Function,
            FunctionType = entity.FunctionType,
            IsEnabled = true,
          }
        );

        await repository.UpdateAsync(condition, true);

        result = true;
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

    public async Task<bool> UpdateRule(Guid conditionId, RuleEntity entity)
    {
      bool result = false;
      try
      {
        var condition = await repository.GetAsync(conditionId, true);

        var rule = condition.Rules.FirstOrDefault(r => r.Id == entity.Id);
        rule.Function = entity.Function;
        rule.FunctionType = entity.FunctionType;
        rule.IsEnabled = true;

        await repository.UpdateAsync(condition, true);

        result = true;
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

    public async Task<ConditionEntity> GetCondition(
      LifecycleConditionTargetType lifecycleConditionTargetType,
      Guid refId
    )
    {
      ConditionEntity result = null;
      try
      {
        result = await repository.GetCondition(lifecycleConditionTargetType, refId);
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
        await repository.DeleteAsync(conditionId, true);
        result = true;
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
        var condition = await repository.GetAsync(conditionId);

        var rule = condition.Rules.FirstOrDefault(r => r.Id == ruleId);
        if (rule == null)
        {
          return result;
        }

        condition.Rules.Remove(rule);

        await repository.UpdateAsync(condition, true);

        result = true;
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
  }
}
