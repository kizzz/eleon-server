using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages;
using Logging.Module;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sentry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Localization;
using VPortal.Lifecycle.Feature.Module.Repositories.Templates;

namespace VPortal.Lifecycle.Feature.Module.DomainServices
{

  public class StatesGroupTemplateDomainService : DomainService
  {
    private readonly IVportalLogger<StatesGroupTemplateDomainService> logger;
    private readonly IStatesGroupTemplatesRepository repository;
    private readonly IDistributedEventBus _eventBus;
    private readonly IdentityUserManager identityUserManager;
    private readonly IdentityRoleManager identityRoleManager;
    private readonly IStringLocalizer<LifecycleFeatureModuleResource> localizer;

    public StatesGroupTemplateDomainService(
        IVportalLogger<StatesGroupTemplateDomainService> logger,
        IStatesGroupTemplatesRepository repository,
        IDistributedEventBus eventBus,
        IdentityUserManager identityUserManager,
        IStringLocalizer<LifecycleFeatureModuleResource> localizer,
IdentityRoleManager identityRoleManager)
    {
      this.logger = logger;
      this.repository = repository;
      _eventBus = eventBus;
      this.identityRoleManager = identityRoleManager;
      this.identityUserManager = identityUserManager;
      this.localizer = localizer;
    }

    public async Task<StatesGroupTemplateEntity> GetAsync(Guid id)
    {

      try
      {
        var stateGroup = await repository.GetAsync(id);
        if (stateGroup == null)
        {
          return null;
        }

        foreach (var state in stateGroup.States ?? new List<StateTemplateEntity>())
        {
          foreach (var stateActor in state.Actors ?? new List<StateActorTemplateEntity>())
          {
            if (stateActor.ActorType == LifecycleActorTypes.Initiator)
            {
              stateActor.DisplayName = localizer["Initiator"];
            }

            if (stateActor.ActorType == LifecycleActorTypes.User)
            {
              var user = await identityUserManager.FindByIdAsync(stateActor.RefId);
              stateActor.DisplayName = user == null ? stateActor.RefId : $"{user.Name} {user.Surname}";
            }

            if (stateActor.ActorType == LifecycleActorTypes.Role)
            {
              var role = await identityRoleManager.FindByNameAsync(stateActor.RefId);
              stateActor.DisplayName = role?.Name ?? stateActor.RefId;
            }

            if (stateActor.ActorType == LifecycleActorTypes.Beneficiary)
            {
              stateActor.DisplayName = localizer["Beneficiary"];
            }
          }
        }
        return stateGroup;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
      return null;
    }

    public async Task<KeyValuePair<long, List<StatesGroupTemplateEntity>>> GetListAsync(
            string documentObjectType,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0)
    {
      KeyValuePair<long, List<StatesGroupTemplateEntity>> result = new();
      try
      {
        result = await repository.GetPaginatedListAsync(documentObjectType, sorting, maxResultCount, skipCount);
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

    public async Task<bool> Add(StatesGroupTemplateEntity statesGroupTemplate)
    {
      bool result = false;
      try
      {
        var count = await repository
            .GetDocumentTypeGroupsCountAsync(statesGroupTemplate.DocumentObjectType);

        result = await repository.Add(statesGroupTemplate);
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

    public async Task<bool> Remove(Guid Id)
    {
      bool result = false;
      try
      {
        await repository.DeleteAsync(Id);
        await _eventBus.PublishAsync(new LifecycleRemovedMsg { LifecylceId = Id, TenantId = CurrentTenant?.Id });
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
        var statesGroup = await repository.GetAsync(id);
        statesGroup.IsActive = newState;
        await repository.UpdateAsync(statesGroup);
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

    public async Task<bool> Rename(Guid id, string newName)
    {
      bool result = false;
      try
      {
        var statesGroup = await repository.GetAsync(id);
        statesGroup.GroupName = newName;
        await repository.UpdateAsync(statesGroup);
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

    public async Task<bool> Update(StatesGroupTemplateEntity statesGroupTemplate)
    {
      bool result = false;
      try
      {
        var existingStatesGroup = await repository.GetAsync(statesGroupTemplate.Id);

        existingStatesGroup.GroupName = statesGroupTemplate.GroupName;
        existingStatesGroup.IsActive = statesGroupTemplate.IsActive;

        await repository.UpdateAsync(existingStatesGroup);

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
