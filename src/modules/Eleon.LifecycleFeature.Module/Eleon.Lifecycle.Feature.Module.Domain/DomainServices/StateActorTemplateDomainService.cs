using Common.Module.Constants;
using Logging.Module;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sentry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Entities.Templates;
using VPortal.Lifecycle.Feature.Module.Localization;
using VPortal.Lifecycle.Feature.Module.Repositories.Templates;

namespace VPortal.Lifecycle.Feature.Module.DomainServices
{

  public class StateActorTemplateDomainService : DomainService
  {
    private readonly IVportalLogger<StateActorTemplateDomainService> _logger;
    private readonly IStatesGroupTemplatesRepository repository;
    private readonly IStringLocalizer<LifecycleFeatureModuleResource> localizer;
    private readonly IdentityUserManager identityUserManager;
    private readonly IdentityRoleManager identityRoleManager;

    public StateActorTemplateDomainService(
        IVportalLogger<StateActorTemplateDomainService> logger,
        IStatesGroupTemplatesRepository repository,
        IStringLocalizer<LifecycleFeatureModuleResource> localizer,
        IdentityUserManager identityUserManager,
        IGuidGenerator guidGenerator,
        IdentityRoleManager identityRoleManager)
    {
      this._logger = logger;
      this.repository = repository;
      this.localizer = localizer;
      this.identityUserManager = identityUserManager;
      this.identityRoleManager = identityRoleManager;
    }

    public async Task<List<StateActorTemplateEntity>> GetAllAsync(Guid stateId)
    {
      List<StateActorTemplateEntity> result = new();
      try
      {
        var state = await repository.GetState(stateId);
        foreach (var stateActor in state.Actors)
        {
          if (stateActor.ActorType == LifecycleActorTypes.Initiator)
          {
            stateActor.DisplayName = localizer["Initiator"];
          }

          if (stateActor.ActorType == LifecycleActorTypes.User)
          {
            var user = await identityUserManager.FindByIdAsync(stateActor.RefId);
            stateActor.DisplayName = $"{user.Name} {user.Surname}";
          }

          if (stateActor.ActorType == LifecycleActorTypes.Role)
          {
            var role = await identityRoleManager.FindByNameAsync(stateActor.RefId);
            stateActor.DisplayName = role.Name;
          }

          if (stateActor.ActorType == LifecycleActorTypes.Beneficiary)
          {
            stateActor.DisplayName = localizer["Beneficiary"];
          }
        }

        result = state.Actors;
      }
      catch (Exception e)
      {
        _logger.Capture(e);
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
        var stateActor = await repository.GetStateActor(id);
        stateActor.IsActive = newState;
        result = true;
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> Add(StateActorTemplateEntity stateActorTemplate)
    {
      bool result = false;
      try
      {
        var state = await repository.GetState(stateActorTemplate.StateTemplateId);
        StateActorTemplateEntity actor = new StateActorTemplateEntity(GuidGenerator.Create());
        actor.ActorName = stateActorTemplate.ActorName;
        actor.ActorType = stateActorTemplate.ActorType;
        actor.DisplayName = stateActorTemplate.DisplayName;
        actor.IsApprovalAdmin = stateActorTemplate.IsApprovalAdmin;
        actor.IsApprovalManager = stateActorTemplate.IsApprovalManager;
        actor.IsApprovalNeeded = stateActorTemplate.IsApprovalNeeded;
        actor.IsFormAdmin = stateActorTemplate.IsFormAdmin;
        actor.OrderIndex = stateActorTemplate.OrderIndex;
        actor.RefId = stateActorTemplate.RefId;
        actor.RuleId = stateActorTemplate.RuleId;
        actor.IsActive = stateActorTemplate.IsActive;
        state.Actors.Add(actor);
        foreach (var taskListTemplate in stateActorTemplate.TaskLists)
        {
          var taskList = new StateActorTaskListSettingTemplateEntity(GuidGenerator.Create());
          taskList.DocumentObjectType = taskListTemplate.DocumentObjectType;
          taskList.TaskListId = taskListTemplate.TaskListId;
          actor.TaskLists.Add(taskList);
        }

        result = true;
      }
      catch (Exception e)
      {
        _logger.Capture(e);
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
        foreach (var stateActorOrder in order)
        {
          var stateActor = await repository.GetStateActor(stateActorOrder.Key);
          if (stateActor != null)
          {
            stateActor.OrderIndex = stateActorOrder.Value;
          }
        }

        result = true;
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> Update(StateActorTemplateEntity stateActorTemplate)
    {
      bool result = false;
      try
      {
        var actor = await repository.GetStateActor(stateActorTemplate.Id);
        actor.ActorName = stateActorTemplate.ActorName;
        actor.ActorType = stateActorTemplate.ActorType;
        actor.DisplayName = stateActorTemplate.DisplayName;
        actor.IsApprovalAdmin = stateActorTemplate.IsApprovalAdmin;
        actor.IsApprovalManager = stateActorTemplate.IsApprovalManager;
        actor.IsApprovalNeeded = stateActorTemplate.IsApprovalNeeded;
        actor.IsFormAdmin = stateActorTemplate.IsFormAdmin;
        actor.OrderIndex = stateActorTemplate.OrderIndex;
        actor.RefId = stateActorTemplate.RefId;
        actor.RuleId = stateActorTemplate.RuleId;
        actor.IsActive = stateActorTemplate.IsActive;
        actor.TaskLists.Clear();
        foreach (var taskListTemplate in stateActorTemplate.TaskLists)
        {
          var taskList = new StateActorTaskListSettingTemplateEntity(GuidGenerator.Create());
          taskList.DocumentObjectType = taskListTemplate.DocumentObjectType;
          taskList.TaskListId = taskListTemplate.TaskListId;
          actor.TaskLists.Add(taskList);
        }
        result = true;
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> Remove(Guid id)
    {
      bool result = false;
      try
      {
        var stateActor = await repository.GetStateActor(id);
        var state = stateActor.StateTemplateEntity;
        state.Actors.Remove(stateActor);
        result = true;
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
  }
}
