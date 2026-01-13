using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class ApiKeyUpdatedMsg : VportalEvent
  {
    public string Name { get; set; }
    public string ApiKeyId { get; set; }
    public bool AllowAuthorize { get; set; }

  }
}
