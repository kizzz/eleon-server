using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class CheckLastLinkActiveMsg : VportalEvent
  {
    public string ObjectType { get; set; }
    public string PrivateParams { get; set; }
  }
}
