using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class CreateBackgroundJobMsg : VportalEvent
  {
    public Guid Id { get; set; }
    public new Guid? TenantId { get; set; }
    public List<Guid> ParentJobsIds { get; set; }
    public string Type { get; set; }
    public string Initiator { get; set; }
    public DateTime ScheduleExecutionDateUtc { get; set; }
    public bool IsRetryAllowed { get; set; }
    public string Description { get; set; }
    public string StartExecutionParams { get; set; }
    public string StartExecutionExtraParams { get; set; }
    public string SourceId { get; set; }
    public string SourceType { get; set; }
    public bool IsSystemInternal { get; set; } = true; // If job created with app service, it is not an internal system job.
    public int TimeoutInMinutes { get; set; }
    public int RetryInMinutes { get; set; }
    public int MaxRetryAttempts { get; set; }
    public string OnFailureRecepients { get; set; }
    public Dictionary<string, string> ExtraProperties { get; set; }
  }
}
