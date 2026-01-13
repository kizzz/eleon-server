using Common.Module.Constants;
using System;
using System.Collections.Generic;
using VPortal.JobScheduler.Module.Actions;
using VPortal.JobScheduler.Module.Triggers;

namespace VPortal.JobScheduler.Module.Tasks
{
  public class TaskDto : TaskHeaderDto
  {
    public List<TaskExecutionDto> Executions { get; set; }
  }
}
