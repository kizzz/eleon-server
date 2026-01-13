using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetCompaniesByOrganizationUnitIdMsg : VportalEvent
  {
    public Guid OrgUnitId { get; set; }
    public bool IsLinked { get; set; }

    public GetCompaniesByOrganizationUnitIdMsg(Guid orgUnitId, bool isLinked)
    {
      OrgUnitId = orgUnitId;
      IsLinked = isLinked;
    }

    public GetCompaniesByOrganizationUnitIdMsg() { }
  }
}
