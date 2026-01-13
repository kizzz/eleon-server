using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class BackgroundJobCompletedMsg : VportalEvent
  {
    public BackgroundJobEto BackgroundJob { get; set; }
    public string CompletedBy { get; set; }
    public bool IsManually { get; set; }
    public Guid JobId { get; set; }
    public BackgroundJobStatus CompletionStatus { get; set; }
  }
}
