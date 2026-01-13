using Common.Module.Constants;
using System;
using System.Collections.Generic;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;

namespace VPortal.Lifecycle.Feature.Module.Dto.Audits
{
  public class StateActorAuditDto
  {
    public Guid Id { get; set; }
    public Guid StateId { get; set; }
    public int? OrderIndex { get; set; }
    public bool IsConditional { get; set; }
    public Guid RuleId { get; set; }
    public bool IsApprovalNeeded { get; set; }
    public bool IsFormAdmin { get; set; }
    public bool IsApprovalManager { get; set; }
    public bool IsApprovalAdmin { get; set; }
    public bool IsActive { get; set; }
    public string ActorName { get; set; }
    public LifecycleActorTypes ActorType { get; set; }
    public string RefId { get; set; }
    public string StatusUserName { get; set; }
    public Guid StatusUserId { get; set; }
    public DateTime StatusDate { get; set; }
    public LifecycleActorStatus Status { get; set; }
    //public List<StateActorTaskListSettingAuditDto> TaskLists { get; set; }
    public string Reason { get; set; }
  }
}
