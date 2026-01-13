using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class OrgUnitByIdGotMsg : VportalEvent
  {
    public Guid Id { get; set; }
    public virtual Guid? ParentId { get; set; }
    public virtual string Code { get; set; }
    public virtual string DisplayName { get; set; }
  }
}
