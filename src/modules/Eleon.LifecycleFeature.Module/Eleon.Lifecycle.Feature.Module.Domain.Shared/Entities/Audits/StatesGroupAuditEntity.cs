using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Lifecycle.Feature.Module.Entities
{
  public class StatesGroupAuditEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual string DocumentObjectType { get; set; }
    public virtual Guid StatesGroupTemplateId { get; set; }
    public virtual string GroupName { get; set; }
    public virtual string DocumentId { get; set; }

    public virtual KeyValuePair<LifecycleFinishedStatus, string> CurrentStatus
    {
      get
      {
        if (Status == LifecycleStatus.New)
        {
          return KeyValuePair.Create(LifecycleFinishedStatus.None, "NotStarted");
        }

        if (Status == LifecycleStatus.Enroute)
        {
          return KeyValuePair.Create(LifecycleFinishedStatus.None, CurrentState.StateName);
        }

        if (States.Any(state => state.CurrentStatus.Key == LifecycleFinishedStatus.Rejected))
        {
          var status = States
                  .Select(state => state.CurrentStatus)
                  .Where(status => status.Key == LifecycleFinishedStatus.Rejected)
                  .FirstOrDefault();

          return status;
        }

        return KeyValuePair.Create(LifecycleFinishedStatus.Approved, string.Empty);
      }
    }

    public virtual LifecycleStatus Status { get; set; }
    public virtual int? CurrentStateOrderIndex { get; set; }
    public virtual StateAuditEntity? CurrentState
    {
      get
      {
        var orderIndex = CurrentStateOrderIndex;
        if (orderIndex == null)
        {
          var newStates = States
              .Where(state => state.Status == LifecycleStatus.Enroute);
          if (newStates.Count() == 0)
          {
            return null;
          }

          orderIndex = newStates.Min(state => state.OrderIndex);
        }

        var state = States.Find(
            state => state.OrderIndex == orderIndex);
        return state;
      }
    }

    public virtual List<StateAuditEntity> States { get; set; }
    public StatesGroupAuditEntity()
        : base()
    {
      States = new List<StateAuditEntity>();
    }

    public StatesGroupAuditEntity(Guid id)
        : base(id)
    {
      States = new List<StateAuditEntity>();
    }
  }
}
