using Common.Module.Constants;
using Logging.Module;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Localization;
using VPortal.Lifecycle.Feature.Module.Repositories.Audits;

namespace VPortal.Lifecycle.Feature.Module.DomainServices
{

  public class StatesGroupAuditDomainService : DomainService
  {
    private readonly IVportalLogger<StatesGroupAuditDomainService> logger;
    private readonly IStatesGroupAuditsRepository repository;
    private readonly IdentityUserManager userManager;
    private readonly ICurrentUser currentUser;
    private readonly IStringLocalizer<LifecycleFeatureModuleResource> localizer;
    private readonly IdentityRoleManager identityRoleManager;

    public StatesGroupAuditDomainService(
        IVportalLogger<StatesGroupAuditDomainService> logger,
        IStatesGroupAuditsRepository repository,
        IdentityUserManager userManager,
        ICurrentUser currentUser,
        IStringLocalizer<LifecycleFeatureModuleResource> localizer,
        IdentityRoleManager identityRoleManager
        )
    {
      this.logger = logger;
      this.repository = repository;
      this.userManager = userManager;
      this.currentUser = currentUser;
      this.localizer = localizer;
      this.identityRoleManager = identityRoleManager;
    }

    public async Task<StatesGroupAuditEntity> GetById(Guid id)
    {
      StatesGroupAuditEntity result = null;
      try
      {
        result = await repository.GetAsync(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
      return result;
    }

    public async Task<bool> Add(StatesGroupAuditEntity statesGroupAudit)
    {
      bool result = false;
      try
      {
        result = await repository.Add(statesGroupAudit);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
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
        result = await repository.Remove(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> DeepCancel(string docType, string documentId)
    {
      bool result = false;
      try
      {
        var entity = await repository.GetByDocIdAsync(docType, documentId);

        if (entity == null)
        {
          return false;
        }

        entity.Status = LifecycleStatus.Canceled;

        foreach (var stateAudit in entity.States)
        {
          stateAudit.Status = LifecycleStatus.Canceled;
          foreach (var actorAudit in stateAudit.Actors)
          {
            actorAudit.Status = LifecycleActorStatus.Canceled;
            actorAudit.StatusDate = DateTime.Now;
          }
        }

        var canceledEntity = await repository.UpdateAsync(entity, true);
        result = canceledEntity != null;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<StatesGroupAuditEntity> ForceComplete(string docType, string documentId)
    {
      StatesGroupAuditEntity result = null;
      try
      {
        var entity = await repository.GetByDocIdAsync(docType, documentId);
        entity.Status = LifecycleStatus.Complete;

        foreach (var stateAudit in entity.States)
        {
          stateAudit.Status = LifecycleStatus.Complete;
          foreach (var actorAudit in stateAudit.Actors)
          {
            actorAudit.Status = LifecycleActorStatus.Canceled;
            actorAudit.StatusDate = DateTime.Now;
          }
        }

        var completedEntity = await repository.UpdateAsync(entity, true);
        result = completedEntity;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<KeyValuePair<long, List<StatesGroupAuditEntity>>> GetReports(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null,
        DateTime? statusDateFilterStart = null,
        DateTime? statusDateFilterEnd = null,
        IList<string>? objectTypeFilter = null,
        Guid? userId = null,
        Guid? statesTemplateGroupId = null)
    {
      KeyValuePair<long, List<StatesGroupAuditEntity>> result = new KeyValuePair<long, List<StatesGroupAuditEntity>>();
      try
      {
        IdentityUser user = null;
        IList<string> roles = null;
        if (userId.HasValue)
        {
          user = await userManager.GetByIdAsync(userId.Value);
          roles = await userManager.GetRolesAsync(user);
        }

        result = await repository.GetReportListsAsync(
            sorting,
            maxResultCount,
            skipCount,
            searchQuery,
            statusDateFilterStart,
            statusDateFilterEnd,
            objectTypeFilter,
            userId,
            roles,
            statesTemplateGroupId);

        foreach (var state in result.Value)
        {
          if (state.CurrentState != null && state.CurrentState.CurrentActor != null)
          {
            await SetActorDisplayName(state.CurrentState.CurrentActor);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<string>> GetDocumentIdsByFilter(string documentObjectType, Guid? userId = null, List<string> roles = null, List<LifecycleStatus> lifecycleStatuses = null)
    {
      List<string> result = new();
      try
      {
        result = await repository.GetDocumentIdsByFilter(documentObjectType, userId, roles, lifecycleStatuses);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    private async Task SetActorDisplayName(StateActorAuditEntity stateActor)
    {
      try
      {
        if (stateActor.ActorType == LifecycleActorTypes.Initiator)
        {
          stateActor.DisplayName = localizer["Initiator"];
        }

        if (stateActor.ActorType == LifecycleActorTypes.User)
        {
          var user = await userManager.FindByIdAsync(stateActor.RefId);
          if (user != null)
          {
            stateActor.DisplayName = $"{user.Name} {user.Surname}";
          }
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
