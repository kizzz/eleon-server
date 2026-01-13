using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class CompaniesByOrgUnitGotMsg : VportalEvent
  {
    public List<string> CompanyUids { get; set; }
  }
}
