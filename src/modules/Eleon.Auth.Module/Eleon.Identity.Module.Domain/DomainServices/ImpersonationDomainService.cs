using Common.EventBus.Module;
using Logging.Module;
using Messaging.Module.Messages;
using Migrations.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace VPortal.Identity.Module.DomainServices
{

  public class ImpersonationDomainService : DomainService
  {
    private readonly IVportalLogger<ImpersonationDomainService> logger;
    private readonly IDistributedEventBus eventBus;
    private readonly IdentityUserManager identityUserManager;

    public ImpersonationDomainService(
        IVportalLogger<ImpersonationDomainService> logger,
        IDistributedEventBus eventBus,
        IdentityUserManager identityUserManager)
    {
      this.logger = logger;
      this.eventBus = eventBus;
      this.identityUserManager = identityUserManager;
    }

    public async Task<bool> CheckIfCanImpersonate(
        Guid? actorTenantId,
        Guid actorUserId,
        Guid? actingAsTenantId,
        Guid actingAsUserId,
        Guid? tryingToImpersonateAsTenantId,
        Guid tryingToImpersonateAsUserId)
    {
      bool result = false;
      try
      {
        bool impersonationPossible = await CheckIfImpersonationIsPossible(
                actorTenantId,
                actorUserId,
                actingAsTenantId,
                actingAsUserId,
                tryingToImpersonateAsTenantId,
                tryingToImpersonateAsUserId);

        if (impersonationPossible)
        {
          result = await CheckIfControlIsDelegated(tryingToImpersonateAsUserId, actorUserId) // important: must be executed first // because it marks last login time for the delegator
              || await CheckIfHasPrivilegedAccess(actorTenantId, actorUserId, tryingToImpersonateAsTenantId, tryingToImpersonateAsUserId);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private async Task<bool> CheckIfImpersonationIsPossible(
        Guid? actorTenantId,
        Guid actorUserId,
        Guid? actingAsTenantId,
        Guid actingAsUserId,
        Guid? tryingToImpersonateAsTenantId,
        Guid tryingToImpersonateAsUserId)
    {
      bool tryingToImpersonateAsSameUser =
          actingAsUserId == tryingToImpersonateAsUserId
          && actingAsTenantId == tryingToImpersonateAsTenantId;

      if (tryingToImpersonateAsSameUser)
      {
        return false;
      }

      bool alreadyImpersonating =
          actorUserId != actingAsUserId
          || actorTenantId != actingAsTenantId;

      bool tryingToImpersonateAsOriginalSelf =
          actorUserId == tryingToImpersonateAsUserId
          && actorTenantId == tryingToImpersonateAsTenantId;

      if (alreadyImpersonating && !tryingToImpersonateAsOriginalSelf)
      {
        return false;
      }

      bool tryingToImpersonateAnotherTenant = actorTenantId != tryingToImpersonateAsTenantId;
      bool isHost = actorTenantId == null;
      if (tryingToImpersonateAnotherTenant && !isHost)
      {
        return false;
      }

      return true;
    }

    private async Task<bool> CheckIfHasPrivilegedAccess(Guid? actorTenantId, Guid actorUserId, Guid? tryingToImpersonateAsTenantId, Guid tryingToImpersonateAsUserId)
    {
      bool tryingToImpersonateAsOriginalSelf = actorUserId == tryingToImpersonateAsUserId && actorTenantId == tryingToImpersonateAsTenantId;
      if (tryingToImpersonateAsOriginalSelf)
      {
        return true;
      }

      bool tryingToImpersonateAnotherTenant = actorTenantId != tryingToImpersonateAsTenantId;
      bool isHost = actorTenantId == null;
      if (tryingToImpersonateAnotherTenant && isHost)
      {
        bool isHostAdmin = await CheckIfIsHostAdmin(actorUserId);
        if (!isHostAdmin)
        {
          return false;
        }
      }

      bool targetUserExists = await CheckIfUserExists(tryingToImpersonateAsTenantId, tryingToImpersonateAsUserId);
      if (!targetUserExists)
      {
        return false;
      }

      return true;
    }

    private async Task<bool> CheckIfIsHostAdmin(Guid userId)
    {
      using (CurrentTenant.Change(null))
      {
        var user = await identityUserManager.GetByIdAsync(userId);
        bool isAdmin = await identityUserManager.IsInRoleAsync(user, MigrationConsts.AdminRoleNameDefaultValue);
        return isAdmin;
      }
    }

    private async Task<bool> CheckIfUserExists(Guid? tenantId, Guid userId)
    {
      using (CurrentTenant.Change(tenantId))
      {
        var user = await identityUserManager.GetByIdAsync(userId);
        return user != null;
      }
    }

    private async Task<bool> CheckIfControlIsDelegated(Guid delegatedByUserId, Guid delegatedToUserId)
    {
      var request = new CheckControlDelegationMsg()
      {
        DelegatedByUserId = delegatedByUserId,
        DelegatedToUserId = delegatedToUserId,
      };

      var response = await eventBus.RequestAsync<ControlDelegationCheckedMsg>(request);
      return response.IsDelegated;
    }
  }
}
