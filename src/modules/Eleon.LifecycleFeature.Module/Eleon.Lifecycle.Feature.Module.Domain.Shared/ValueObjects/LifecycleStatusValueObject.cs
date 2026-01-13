using Common.Module.Constants;
using System;

namespace VPortal.Lifecycle.Feature.Module.ValueObjects
{
  public class LifecycleStatusValueObject
  {
    public LifecycleStatus LifecycleStatus { get; set; }
    public DateTime? StatusDate { get; set; }
    public LifecycleActorStatus ActorStatus { get; set; }
    public string ActorName { get; set; }
    public LifecycleActorTypes ActorType { get; set; }
    public LifecycleApprovalType LifecycleApprovalType { get; set; }
    public string RejectedReason { get; set; }
  }
}
