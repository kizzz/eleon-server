using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class AddApiKeyMsg : VportalEvent
  {
    public string ApiKey { get; set; }
    public string Subject { get; set; }
    public ApiKeyType Type { get; set; }
    public string Name { get; set; }
    public bool AllowAuthorize { get; set; }
    public string Data { get; set; }
    public DateTime? ExpiresAt { get; set; }
  }
}
