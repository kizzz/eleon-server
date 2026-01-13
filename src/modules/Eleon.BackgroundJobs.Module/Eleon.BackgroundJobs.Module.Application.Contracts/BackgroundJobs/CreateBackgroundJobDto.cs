using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundJobs.Module.BackgroundJobs;
public class CreateBackgroundJobDto
{
  public List<Guid> ParentJobsIds { get; set; }
  public string Type { get; set; }
  public string Initiator { get; set; }
  public DateTime ScheduleExecutionDateUtc { get; set; }
  public bool IsRetryAllowed { get; set; }
  public string Description { get; set; }
  public string StartExecutionParams { get; set; }
  public string StartExecutionExtraParams { get; set; }
  public int TimeoutInMinutes { get; set; }
  public int RetryInMinutes { get; set; }
  public int MaxRetryAttempts { get; set; }
  public string OnFailureRecepients { get; set; }
}
