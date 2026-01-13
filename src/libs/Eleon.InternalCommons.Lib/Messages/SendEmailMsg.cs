using Messaging.Module.ETO;
using System.ComponentModel.DataAnnotations;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendEmailMsg : VportalEvent
  {
    public string SenderEmailAddress { get; set; }

    public string TargetEmailAddress { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }
  }
}
