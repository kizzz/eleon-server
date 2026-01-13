using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class PushChatMessageMsg : VportalEvent
  {
    public ChatPushMessageEto Message { get; set; }
    public List<Guid> AudienceUserIds { get; set; }
    public List<string> AudienceRoles { get; set; }
    public List<Guid> AudienceOrgUnits { get; set; }
  }
}
