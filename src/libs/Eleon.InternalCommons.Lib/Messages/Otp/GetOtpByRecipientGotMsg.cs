using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetOtpByRecipientGotMsg
  {
    public string Recipient { get; set; }
    public bool IsExpired { get; set; }

    public GetOtpByRecipientGotMsg()
    {
    }
  }
}
