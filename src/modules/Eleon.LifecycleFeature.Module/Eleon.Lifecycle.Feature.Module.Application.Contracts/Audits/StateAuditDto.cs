using Common.Module.Constants;
using System;

namespace VPortal.Lifecycle.Feature.Module.Dto.Audits
{
  public class StateAuditDto
  {
    public Guid Id { get; set; }
    public Guid StatesGroupId { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; }
    public string StateName { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsReadOnly { get; set; }
    public int ApprovalType { get; set; }
    public int? CurrentActorOrderIndex { get; set; }
    public LifecycleStatus Status { get; set; }
    public CurrentStatusDto CurrentStatus { get; set; }
    public DateTime CreationTime { get; set; }
  }
}
