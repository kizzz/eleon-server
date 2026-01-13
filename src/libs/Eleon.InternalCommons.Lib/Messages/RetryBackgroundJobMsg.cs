namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class RetryBackgroundJobMsg : VportalEvent
  {
    public Guid JobId { get; set; }
    public string StartExecutionParams { get; set; }
    public string StartExecutionExtraParams { get; set; }
    public int TimeoutInMinutes { get; set; }
    public int RetryInMinutes { get; set; }
    public int MaxRetryAttempts { get; set; }
    public string OnFailureRecepients { get; set; }
  }
}
