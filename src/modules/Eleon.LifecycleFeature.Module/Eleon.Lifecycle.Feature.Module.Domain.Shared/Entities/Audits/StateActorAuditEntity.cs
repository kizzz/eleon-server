using Common.Module.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Lifecycle.Feature.Module.Entities
{
  public class StateActorAuditEntity : Entity<Guid>, IHasCreationTime, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual DateTime CreationTime { get; set; }
    [NotMapped]
    public virtual string DisplayName { get; set; }
    public virtual string ActorName { get; set; }
    public virtual Guid StateId { get; set; }
    public virtual StateAuditEntity State { get; set; }
    public virtual Guid StateActorTemplateId { get; set; }
    public virtual int? OrderIndex { get; set; }
    public virtual bool IsConditional { get; set; }
    public virtual Guid RuleId { get; set; }
    public virtual bool IsApprovalNeeded { get; set; }
    public virtual bool IsFormAdmin { get; set; }
    public virtual bool IsApprovalManager { get; set; }
    public virtual bool IsApprovalAdmin { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual LifecycleActorTypes ActorType { get; set; }
    public virtual string RefId { get; set; }
    public virtual string StatusUserName { get; set; }
    public virtual Guid StatusUserId { get; set; }
    public virtual DateTime StatusDate { get; set; }
    public virtual LifecycleActorStatus Status { get; set; }
    public virtual bool IsEnroute
    {
      get
      {
        return (Status == LifecycleActorStatus.Enroute
            || Status == LifecycleActorStatus.Reviewed)
            && IsApprovalNeeded && IsActive;
      }
    }
    public virtual string Reason { get; set; }

    public StateActorAuditEntity()
        : base()
    {
    }

    public StateActorAuditEntity(Guid id)
        : base(id)
    {
    }
  }
}
