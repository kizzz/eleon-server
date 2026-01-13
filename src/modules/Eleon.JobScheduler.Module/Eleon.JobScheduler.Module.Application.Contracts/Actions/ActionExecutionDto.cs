using Common.Module.Constants;
using System;
using System.Collections.Generic;

namespace VPortal.JobScheduler.Module.Actions
{
  public class ActionExecutionDto
  {
    public Guid Id { get; set; }
    public JobSchedulerActionExecutionStatus Status { get; set; }
    public Guid? JobId { get; set; }
    public List<Guid> ParentActionExecutionIds { get; set; }
    public string EventName { get; set; }
    public string ActionName { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string StatusChangedBy { get; set; }
    public bool IsStatusChangedManually { get; set; }
  }
}
