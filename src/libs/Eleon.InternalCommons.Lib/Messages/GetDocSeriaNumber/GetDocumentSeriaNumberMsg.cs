using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetDocumentSeriaNumberMsg : VportalEvent
  {
    public string ObjectType { get; set; }

    public string RefId { get; set; }
    public string Prefix { get; set; }
  }
}
