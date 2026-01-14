using Common.Module.Constants;

namespace Messaging.Module.ETO
{
  public class BackgroundJobEto
  {
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? ParentJobId { get; set; }
    public BackgroundJobStatus Status { get; set; }
    public string Type { get; set; }
    public DateTime? LastExecutionDateUtc { get; set; }
    public DateTime ScheduleExecutionDateUtc { get; set; }
    public string Initiator { get; set; }
    public bool IsRetryAllowed { get; set; }
    public string Description { get; set; }
    public string StartExecutionParams { get; set; }
    public string StartExecutionExtraParams { get; set; }
    public Dictionary<string, string> ExtraProperties { get; set; }
  }
}
