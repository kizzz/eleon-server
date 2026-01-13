using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetUserSignInSettingMsg : VportalEvent
  {
    public Guid UserId { get; set; }

    public GetUserSignInSettingMsg(Guid userId)
    {
      UserId = userId;
    }

    public GetUserSignInSettingMsg() { }
  }
}
