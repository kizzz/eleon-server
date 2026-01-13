using Common.Module.Events;
using Messaging.Module.Messages;

namespace SharedCollector.deprecated.Messaging.Module.SystemMessages;

[DistributedEvent]
public class CustomSmsMsg : VportalEvent
{
  public string Sender { get; set; }
  public string PhoneNumber { get; set; }
  public string Message { get; set; }
  public string Type { get; set; }
  public DateTime SendTimeUTC { get; set; }
}
