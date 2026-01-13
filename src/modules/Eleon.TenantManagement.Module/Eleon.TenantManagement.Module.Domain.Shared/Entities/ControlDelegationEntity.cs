using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.TenantManagement.Module.Entities
{
  public class ControlDelegationEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid DelegatedToUserId { get; set; }
    public DateTime DelegationStartDate { get; set; }
    public DateTime? DelegationEndDate { get; set; }
    public bool Active { get; set; } = true;
    public string Reason { get; set; }
    public DateTime? LastLoginDate { get; set; }

    [NotMapped]
    public string UserName { get; set; }

    [NotMapped]
    public string DelegatedToUserName { get; set; }

    public virtual List<ControlDelegationHistoryEntity> DelegationHistory { get; set; }

    public ControlDelegationEntity(Guid id, Guid userId, Guid delegatedToUserId, DateTime delegationStartDate, DateTime? delegationEndDate = null, string reason = null)
    {
      Id = id;
      UserId = userId;
      DelegatedToUserId = delegatedToUserId;
      DelegationStartDate = delegationStartDate;
      DelegationEndDate = delegationEndDate;
      Reason = reason;
    }
  }
}
