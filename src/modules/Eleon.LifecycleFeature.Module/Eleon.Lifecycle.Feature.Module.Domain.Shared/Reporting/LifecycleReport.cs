using Common.Module.Constants;
using System;
using System.Collections.Generic;
using VPortal.Lifecycle.Feature.Module.Specifications;

namespace VPortal.Lifecycle.Feature.Module.Reporting
{
  public class LifecycleReport : ILifecycleReportable
  {
    public Guid LifecycleInitiatorId { get; set; }
    public LifecycleApprovalType? ApprovalType { get; set; }
    public int? ActorOrderIndex { get; set; }
    public int? StateOrderIndex { get; set; }
    public int? CurrentActorOrderIndex { get; set; }
    public int? CurrentStateOrderIndex { get; set; }
    public LifecycleStatus? LifecycleStateStatus { get; set; }
    public LifecycleStatus? LifecycleGroupStatus { get; set; }
    public LifecycleActorStatus? LifecycleActorStatus { get; set; }
    public LifecycleActorTypes? ActorType { get; set; }
    public string ActorName { get; set; }
    public string ActorRefId { get; set; }
    public bool? IsApprovalNeeded { get; set; }
    public bool IsUnique { get; set; }

    public void CleanLifecycleInfoIfInitiator(Guid userId, IEnumerable<string> roles)
    {
      var spec = new ActorSpecification<LifecycleReport>(userId, roles);
      if (!spec.IsSatisfiedBy(this))
      {
        ActorRefId = null;
        ActorName = null;
        IsApprovalNeeded = null;
        ActorType = null;
        ActorOrderIndex = null;
        StateOrderIndex = null;
        CurrentActorOrderIndex = null;
        CurrentStateOrderIndex = null;
        LifecycleStateStatus = null;
        LifecycleGroupStatus = null;
        LifecycleActorStatus = null;
      }
    }
  }
}
