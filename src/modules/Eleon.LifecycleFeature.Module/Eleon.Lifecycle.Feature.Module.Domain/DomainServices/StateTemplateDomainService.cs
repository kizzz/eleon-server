using Common.Module.Constants;
using Logging.Module;
using Microsoft.Extensions.Logging;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Repositories.Templates;

namespace VPortal.Lifecycle.Feature.Module.DomainServices
{

  public class StateTemplateDomainService : DomainService
  {
    private readonly IVportalLogger<StateTemplateDomainService> logger;
    private readonly IStatesGroupTemplatesRepository repository;

    public StateTemplateDomainService(
        IVportalLogger<StateTemplateDomainService> logger,
        IStatesGroupTemplatesRepository repository)
    {
      this.logger = logger;
      this.repository = repository;
    }

    public async Task<List<StateTemplateEntity>> GetAllAsync(Guid groupId)
    {
      List<StateTemplateEntity> result = new();
      try
      {
        var group = await repository.GetAsync(groupId);
        result = group.States;
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

    public async Task<bool> UpdateOrderIndex(Guid id, int orderIndex)
    {
      bool result = false;
      try
      {
        var stateTemplate = await repository.GetState(id);
        stateTemplate.OrderIndex = orderIndex;
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

    public async Task<bool> Enable(Guid id, bool newState)
    {
      bool result = false;
      try
      {
        var stateTemplate = await repository.GetState(id);
        stateTemplate.IsActive = newState;
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

    public async Task<bool> UpdateApprovalType(Guid id, LifecycleApprovalType newApprovalType)
    {
      bool result = false;
      try
      {
        var stateTemplate = await repository.GetState(id);
        stateTemplate.ApprovalType = newApprovalType;
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

    public async Task<bool> UpdateName(Guid id, string name)
    {
      bool result;
      try
      {
        var stateTemplate = await repository.GetState(id);
        stateTemplate.StateName = name;
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
      return result;

    }

    public async Task<bool> Add(StateTemplateEntity stateTemplateEntity)
    {
      bool result = false;
      try
      {
        var group = await repository.GetAsync(stateTemplateEntity.StatesGroupTemplateId);
        group.States.Add(stateTemplateEntity);
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

    public async Task<bool> UpdateOrderIndexes(Dictionary<Guid, int> order)
    {
      bool result = false;
      try
      {
        foreach (var stateOrder in order)
        {
          var state = await repository.GetState(stateOrder.Key);
          if (state != null)
          {
            state.OrderIndex = stateOrder.Value;
          }
        }

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

    public async Task<bool> Remove(Guid groupId, Guid stateId)
    {
      bool result = false;
      try
      {
        var stateTemplate = await repository.GetAsync(groupId);
        var state = stateTemplate.States.First(x => x.Id == stateId);
        stateTemplate.States.Remove(state);
        await repository.UpdateAsync(stateTemplate, true);
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
