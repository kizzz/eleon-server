using Logging.Module;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Localization;
using VPortal.TenantManagement.Module.Repositories;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public class ControlDelegationDomainService : DomainService
  {
    private readonly IVportalLogger<ControlDelegationDomainService> logger;
    private readonly ICurrentUser currentUser;
    private readonly IStringLocalizer<TenantManagementResource> stringLocalizer;
    private readonly ICommonUsersRepository commonUsersRepository;
    private readonly IControlDelegationRepository controlDelegationRepository;

    public ControlDelegationDomainService(
        IVportalLogger<ControlDelegationDomainService> logger,
        ICurrentUser currentUser,
        IStringLocalizer<TenantManagementResource> stringLocalizer,
        ICommonUsersRepository commonUsersRepository,
        IControlDelegationRepository controlDelegationRepository)
    {
      this.logger = logger;
      this.currentUser = currentUser;
      this.stringLocalizer = stringLocalizer;
      this.commonUsersRepository = commonUsersRepository;
      this.controlDelegationRepository = controlDelegationRepository;
    }

    public async Task AddControlDelegation(Guid delegateToUserId, DateTime fromDate, DateTime? toDate, string reason)
    {
      try
      {
        if (delegateToUserId == currentUser.Id.Value)
        {
          throw new UserFriendlyException(stringLocalizer["ControlDelegation:Error:CanNotDelegateToSelf"]);
        }

        var delegation = new ControlDelegationEntity(GuidGenerator.Create(), currentUser.Id.Value, delegateToUserId, fromDate, delegationEndDate: toDate, reason: reason);
        await controlDelegationRepository.InsertAsync(delegation);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task UpdateControlDelegation(Guid delegationId, DateTime fromDate, DateTime? toDate, string reason)
    {
      try
      {
        var delegation = await GetAsOwner(delegationId);
        delegation.DelegationStartDate = fromDate;
        delegation.DelegationEndDate = toDate;
        delegation.Reason = reason;
        await controlDelegationRepository.UpdateAsync(delegation);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task SetControlDelegationActiveState(Guid delegationId, bool isActive)
    {
      try
      {
        var delegation = await GetAsOwner(delegationId);
        delegation.Active = isActive;
        await controlDelegationRepository.UpdateAsync(delegation);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<ControlDelegationEntity>> GetActiveControlDelegationsByUser()
    {
      List<ControlDelegationEntity> result = null;
      try
      {
        var now = DateTime.UtcNow;
        result = await controlDelegationRepository.GetActiveControlDelegationsByUser(currentUser.Id.Value, now);
        await FillUserNames(result);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<ControlDelegationEntity>> GetActiveControlDelegationsToUser()
    {
      List<ControlDelegationEntity> result = null;
      try
      {
        var now = DateTime.UtcNow;
        result = await controlDelegationRepository.GetActiveControlDelegationsToUser(currentUser.Id.Value, now);
        await FillUserNames(result);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<KeyValuePair<int, List<ControlDelegationEntity>>> GetControlDelegationsByUser(int skip, int take)
    {
      KeyValuePair<int, List<ControlDelegationEntity>> result = default;
      try
      {
        var now = DateTime.UtcNow;
        result = await controlDelegationRepository.GetControlDelegationsByUser(currentUser.Id.Value, skip, take);
        await FillUserNames(result.Value);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ControlDelegationEntity> GetControlDelegation(Guid delegationId)
    {
      ControlDelegationEntity result = null;
      try
      {
        result = await GetAsOwner(delegationId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task RemoveControlDelegationsRelatedToUser(Guid userId)
    {
      try
      {
        var delegations = await controlDelegationRepository.GetControlDelegationsRelatedToUser(userId);
        await controlDelegationRepository.DeleteManyAsync(delegations);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<bool> IsControlDelegated(Guid delegatedByUserId, Guid delegatedToUserId)
    {
      bool result = false;
      try
      {
        var now = DateTime.UtcNow;
        var userDelegations = await controlDelegationRepository.GetActiveControlDelegationsByUser(delegatedByUserId, now);
        var delegationToUser = userDelegations.FirstOrDefault(x => x.DelegatedToUserId == delegatedToUserId);
        if (delegationToUser != null)
        {
          delegationToUser.LastLoginDate = now;
          await controlDelegationRepository.UpdateAsync(delegationToUser);
          result = true;
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private async Task<ControlDelegationEntity> GetAsOwner(Guid delegationId)
    {
      var delegation = await controlDelegationRepository.GetAsync(delegationId);
      if (delegation.UserId != currentUser.Id.Value)
      {
        throw new Exception("Unable to get control delegation for another user");
      }

      return delegation;
    }

    private async Task FillUserNames(List<ControlDelegationEntity> delegations)
    {
      var userIds = delegations
          .Select(x => x.UserId)
          .Concat(delegations.Select(x => x.DelegatedToUserId))
          .Distinct()
          .ToList();

      var users = await commonUsersRepository.GetByIds(userIds);
      foreach (var delegation in delegations)
      {
        delegation.UserName = users.FirstOrDefault(x => x.Id == delegation.UserId)?.UserName;
        delegation.DelegatedToUserName = users.FirstOrDefault(x => x.Id == delegation.DelegatedToUserId)?.UserName;
      }
    }
  }
}
