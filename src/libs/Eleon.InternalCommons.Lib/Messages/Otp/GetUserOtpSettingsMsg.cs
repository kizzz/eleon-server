using Common.Module.Constants;
using Common.Module.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.Messages
{
  [DistributedEvent]
  public class GetUserOtpSettingsMsg : VportalEvent
  {
    public Guid UserId { get; set; }
  }
}
