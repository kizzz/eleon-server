using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.Messages
{
  [DistributedEvent]
  public class EventBusCheckedMsg : VportalEvent
  {
  }
}
