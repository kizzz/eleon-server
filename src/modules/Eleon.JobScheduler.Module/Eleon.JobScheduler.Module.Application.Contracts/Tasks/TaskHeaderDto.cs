using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.JobScheduler.Module.Tasks;
public class TaskHeaderDto
{
  public Guid Id { get; set; }
  public bool IsActive { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public bool CanRunManually { get; set; }
  public int? RestartAfterFailIntervalSeconds { get; set; }
  public int RestartAfterFailMaxAttempts { get; set; }
  public int? TimeoutSeconds { get; set; }
  public bool AllowForceStop { get; set; }
  public DateTime? LastRunTimeUtc { get; set; }
  public DateTime? NextRunTimeUtc { get; set; }
  public JobSchedulerTaskStatus Status { get; set; }
  public int? LastDurationSeconds { get; set; }
  public string OnFailureRecepients { get; set; }

}
