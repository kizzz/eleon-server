using Common.Module.Constants;
using System;

namespace BackgroundJobs.Module.BackgroundJobs
{
  public class BackgroundJobHeaderDto
  {
    public Guid Id { get; set; }
    public Guid? ParentJobId { get; set; }
    public DateTime CreationTime { get; set; }
    public string Type { get; set; }
    public string Initiator { get; set; }
    public BackgroundJobStatus Status { get; set; }
    public DateTime ScheduleExecutionDateUtc { get; set; }
    public DateTime? JobFinishedUtc { get; set; }
    public DateTime LastExecutionDateUtc { get; set; }
    public bool IsRetryAllowed { get; set; }
    public string Description { get; set; }
    public string SourceId { get; set; }
    public string SourceType { get; set; }
    public int TimeoutInMinutes { get; set; }

    public int RetryIntervalInMinutes { get; set; }
    public int MaxRetryAttempts { get; set; }
    public int CurrentRetryAttempt { get; set; }
    public DateTime? NextRetryTimeUtc { get; set; }
    public string OnFailureRecepients { get; set; }
  }
}
