using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class ValidateOtpMsg : VportalEvent
  {
    public string Key { get; set; }
    public string Password { get; set; }
  }
}
