using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class SetUserSessionStateMsg : VportalEvent
  {
    public Guid UserId { get; set; }

    public bool RequirePeriodicPasswordChange { get; set; }
    public bool PermissionErrorEncountered { get; set; }

  }
}
