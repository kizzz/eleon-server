using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetUserSignInSettingGotMsg : VportalEvent
  {
    public Guid UserId { get; set; }
    public TwoFaNotificationType? TwoFaNotificationType { get; set; }

    public GetUserSignInSettingGotMsg() { }
  }
}
