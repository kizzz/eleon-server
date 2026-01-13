using Messaging.Module.ETO;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class CancelBackgroundJobMsg : VportalEvent
  {
    public Guid JobId { get; set; }
    public string CancelledBy { get; set; }
    public bool IsManually { get; set; }
    public required string CancelledMessage { get; set; }
  }
}
