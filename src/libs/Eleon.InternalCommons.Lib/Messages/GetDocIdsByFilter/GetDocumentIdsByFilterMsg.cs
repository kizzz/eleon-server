using Common.Module.Constants;
using Common.Module.Serialization;

namespace Messaging.Module.Messages
{
  public class GetDocumentIdsByFilterMsg : VportalEvent
  {
    public string DocumentObjectType { get; set; }
    public List<LifecycleStatus> LifecycleStatuses { get; set; }
    public List<string> Roles { get; set; }
    public Guid? UserId { get; set; }
  }
}
