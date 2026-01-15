using Common.Module.Constants;
using System;
using System.Collections.Generic;
using VPortal.JobScheduler.Module.Actions;

namespace VPortal.JobScheduler.Module.Tasks;

public class TaskExecutionDto
{
  public Guid Id { get; set; }
  public JobSchedulerTaskExecutionStatus Status { get; set; }
  public Guid? RunnedByUserId { get; set; }
  public string RunnedByUserName { get; set; }
  public Guid? RunnedByTriggerId { get; set; }
  public string RunnedByTriggerName { get; set; }
  public DateTime? StartedAtUtc { get; set; }
  public DateTime? FinishedAtUtc { get; set; }
  public Guid TaskId { get; set; }
  public List<ActionExecutionDto> ActionExecutions { get; set; }
}
