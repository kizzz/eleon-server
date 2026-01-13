using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class JobSchedulerTaskExecutionCanceledMsg : VportalEvent
  {
    public Guid TaskExecutionId { get; set; }
    public Guid TaskId { get; set; }
  }
}
