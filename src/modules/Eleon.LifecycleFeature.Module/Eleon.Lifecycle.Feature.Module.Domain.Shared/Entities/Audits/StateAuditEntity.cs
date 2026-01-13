using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Lifecycle.Feature.Module.Entities
{
  public class StateAuditEntity : Entity<Guid>, IHasCreationTime, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual DateTime CreationTime { get; set; }
    public virtual Guid StatesGroupId { get; set; }
    public virtual StatesGroupAuditEntity StatesGroupAudit { get; set; }
    public virtual Guid StatesTemplateId { get; set; }
    public virtual int OrderIndex { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual string StateName { get; set; }
    public virtual bool IsMandatory { get; set; }
    public virtual bool IsReadOnly { get; set; }
    public virtual LifecycleStatus Status { get; set; }
    public virtual LifecycleApprovalType ApprovalType { get; set; }
    public virtual int? CurrentActorOrderIndex { get; set; }
    public virtual StateActorAuditEntity? CurrentActor
    {
      get
      {
        var orderIndex = CurrentActorOrderIndex;
        if (orderIndex == null)
        {
          var newActors = Actors
              .Where(actor => actor.IsEnroute);
          if (newActors.Count() == 0)
          {
            return null;
          }
          orderIndex = newActors.Min(actor => actor.OrderIndex);
        }

        return Actors.Find(
            actor => actor.OrderIndex == orderIndex);
      }
    }

    public virtual KeyValuePair<LifecycleFinishedStatus, string> CurrentStatus
    {
      get
      {
        if (Status == LifecycleStatus.Enroute)
        {
          return KeyValuePair.Create(LifecycleFinishedStatus.None, CurrentActor?.ActorName);
        }

        if (Actors.Any(actor => actor.Status == LifecycleActorStatus.Rejected))
        {
          var actor = Actors
                  .Where(actor => actor.Status == LifecycleActorStatus.Rejected)
                  .FirstOrDefault();

          return KeyValuePair.Create(LifecycleFinishedStatus.Rejected, actor.StatusUserName);
        }

        return KeyValuePair.Create(LifecycleFinishedStatus.Approved, string.Empty);
      }
    }

    public virtual List<StateActorAuditEntity> Actors { get; set; }
    public StateAuditEntity()
        : base()
    {
      Actors = new List<StateActorAuditEntity>();
    }
    public StateAuditEntity(Guid id)
        : base(id)
    {
      Actors = new List<StateActorAuditEntity>();
    }

    public virtual DateTime? LastStatusDate { get; set; }
  }
}
