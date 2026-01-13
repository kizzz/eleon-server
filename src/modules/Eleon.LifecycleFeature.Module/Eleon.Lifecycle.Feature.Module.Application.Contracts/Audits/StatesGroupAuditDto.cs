using Common.Module.Constants;
using System;

namespace VPortal.Lifecycle.Feature.Module.Dto.Audits
{
  public class StatesGroupAuditDto
  {
    public Guid Id { get; set; }
    public string DocumentId { get; set; }
    public string DocumentObjectType { get; set; }
    public string GroupName { get; set; }
    public int? CurrentStateOrderIndex { get; set; }
    public Guid CreatorId { get; set; }
    public LifecycleStatus Status { get; set; }
    public Guid StatesGroupTemplateId { get; set; }
    public CurrentStatusDto CurrentStatus { get; set; }
  }
}
