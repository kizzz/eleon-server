using Common.Module.Constants;

namespace VPortal.Lifecycle.Feature.Module.Reporting
{
  public interface ILifecycleReportable
  {
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
  }
}
