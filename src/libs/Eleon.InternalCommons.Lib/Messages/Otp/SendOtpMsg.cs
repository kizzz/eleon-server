using Common.Module.Constants;
using EleonsoftAbp.EleonsoftIdentity.Sessions;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SendOtpMsg : VportalEvent
  {
    public Guid? UserId { get; set; }
    public string Key { get; set; }
    public List<OtpRecepientEto> Recipients { get; set; }
    public string MessageText { get; set; }
    public FullSessionInformation Session { get; set; }
  }
}
