using Common.Module.Constants;
using System;

namespace VPortal.Lifecycle.Feature.Module.Dto.Templates
{
  public class StateTemplateDto
  {
    public Guid Id { get; set; }
    public Guid StatesGroupTemplateId { get; set; }
    public bool IsActive { get; set; }
    public int? OrderIndex { get; set; }
    public string StateName { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsReadOnly { get; set; }
    public LifecycleApprovalType ApprovalType { get; set; }
    public DateTime CreationTime { get; set; }
  }
}
