using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class ResetTenantAdminPasswordMsg : VportalEvent
  {
    public string ObjectType { get; set; }
    public string DocumentId { get; set; }

    public string AccountXml { get; set; }
  }
}
