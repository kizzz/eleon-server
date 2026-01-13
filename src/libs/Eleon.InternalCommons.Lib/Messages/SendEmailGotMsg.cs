using Messaging.Module.ETO;
using System.ComponentModel.DataAnnotations;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendEmailGotMsg : VportalEvent
  {
    public string ErrorMsg { get; set; }
  }
}
