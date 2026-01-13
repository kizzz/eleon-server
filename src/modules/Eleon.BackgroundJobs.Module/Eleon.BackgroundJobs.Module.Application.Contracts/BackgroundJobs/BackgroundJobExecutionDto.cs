using Common.Module.Constants;
using System;
using System.Collections.Generic;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.BackgroundJobs
{
  public class BackgroundJobExecutionDto
  {
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime ExecutionStartTimeUtc { get; set; }
    public DateTime? ExecutionEndTimeUtc { get; set; }
    public BackgroundJobExecutionStatus Status { get; set; }
    public bool IsRetryExecution { get; set; }
    public Guid? RetryUserInitiatorId { get; set; }
    public string StartExecutionParams { get; set; }
    public string StartExecutionExtraParams { get; set; }
    public Guid BackgroundJobEntityId { get; set; }
    public virtual List<BackgroundJobMessageDto> Messages { get; set; }
    public string StatusChangedBy { get; set; }
    public bool IsStatusChangedManually { get; set; }
  }
}
