using Common.Module.Events;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.BackgroundJobs;
[DistributedEvent]
public class MarkJobExecutionStartedMsg : VportalEvent
{
  public Guid JobId { get; set; }
  public Guid ExecutionId { get; set; }
}
