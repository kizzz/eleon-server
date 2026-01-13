using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class BackgroundJobExecutionCompletedMsg : VportalEvent
  {
    public string Type { get; set; }
    public Guid BackgroundJobId { get; set; }
    public Guid ExecutionId { get; set; }

    public string ParamsForRetryExecution { get; set; }
    public string ExtraParamsForRetryExecution { get; set; }

    public BackgroundJobExecutionStatus Status { get; set; }
    public List<BackgroundJobTextInfoEto> Messages { get; set; }
    public string Result { get; set; }
    public string CompletedBy { get; set; }
    public bool IsManually { get; set; }
  }
}
