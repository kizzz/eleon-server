using Common.Module.Events;

namespace EleonsoftSdk.Messages.User;

[DistributedEvent]
public class GetAllUsersResponseMsg
{
  public List<EleoncoreUserEto> Users { get; set; } = new List<EleoncoreUserEto>();
}
