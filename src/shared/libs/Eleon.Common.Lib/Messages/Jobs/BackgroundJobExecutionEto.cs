using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages;
public class BackgroundJobExecutionEto
{
  public Guid Id { get; set; }
  public DateTime CreationTime { get; set; }
  public DateTime ExecutionStartTimeUtc { get; set; }
  public DateTime? ExecutionEndTimeUtc { get; set; }
  public BackgroundJobExecutionStatus Status { get; set; }
  public bool IsRetryExecution { get; set; }
  public Guid? RetryUserInitiatorId { get; set; }
  public string StartExecutionParams { get; set; }
  public string StartExecutionExtraParams { get; set; }
  public Guid BackgroundJobEntityId { get; set; }
}
