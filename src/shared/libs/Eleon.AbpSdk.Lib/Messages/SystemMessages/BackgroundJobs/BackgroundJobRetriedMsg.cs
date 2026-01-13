namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class BackgroundJobRetriedMsg : VportalEvent
  {
    public Guid JobId { get; set; }
    public required string JobType { get; set; }
    public required string StartExectuionExtraParams { get; set; }
  }
}
