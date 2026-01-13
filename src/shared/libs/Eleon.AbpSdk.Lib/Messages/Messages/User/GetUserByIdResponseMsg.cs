using Common.Module.Events;

namespace EleonsoftSdk.Messages.User;

[DistributedEvent]
public class GetUserByIdResponseMsg
{
  public EleoncoreUserEto User { get; set; }
}
