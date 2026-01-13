using System.Collections.Generic;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;

namespace VPortal.Lifecycle.Feature.Module.LifecycleManager
{
  public class StatesGroupAuditTreeDto : StatesGroupAuditDto
  {
    public List<StateAuditTreeDto> States { get; set; }
    public StateAuditTreeDto? CurrentState { get; set; }
    public Dictionary<string, object> ExtraProperties { get; set; }
  }
}
