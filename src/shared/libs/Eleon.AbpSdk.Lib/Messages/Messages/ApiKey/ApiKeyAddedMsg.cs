using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class ApiKeyAddedMsg : VportalEvent
  {
    public bool AddedSuccessfully { get; set; }
    public Guid ApiKeyId { get; set; }
  }
}
