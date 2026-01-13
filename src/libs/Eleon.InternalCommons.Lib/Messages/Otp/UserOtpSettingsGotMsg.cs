using Common.Module.Events;
using Messaging.Module.ETO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.Messages
{
  [DistributedEvent]
  public class UserOtpSettingsGotMsg : VportalEvent
  {
    public UserOtpSettingsEto Settings { get; set; }
  }
}
