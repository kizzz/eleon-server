using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class DeleteIdentityApiKeyMsg : VportalEvent
  {
    public string Subject { get; set; }
    public ApiKeyType Type { get; set; }
  }
}
