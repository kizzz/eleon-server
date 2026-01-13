using Common.Module.Constants;
using System;
using System.Collections.Generic;

namespace VPortal.Lifecycle.Feature.Module.Dto.Templates
{
  public class StateActorTemplateDto
  {
    public Guid Id { get; set; }
    public Guid StateTemplateId { get; set; }
    public string ActorName { get; set; }
    public int? OrderIndex { get; set; }
    public string RefId { get; set; }
    public LifecycleActorTypes ActorType { get; set; }
    public bool IsConditional { get; set; }
    public Guid RuleId { get; set; }
    public bool IsApprovalNeeded { get; set; }
    public bool IsFormAdmin { get; set; }
    public bool IsApprovalManager { get; set; }
    public bool IsApprovalAdmin { get; set; }
    public bool IsActive { get; set; }
    public List<StateActorTaskListSettingTemplateDto> TaskLists { get; set; }
    public string DisplayName { get; set; }
  }
}
