using System.Collections.Generic;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;

namespace VPortal.Lifecycle.Feature.Module.LifecycleManager
{
  public class StateAuditTreeDto : StateAuditDto
  {
    public List<StateActorAuditTreeDto> Actors { get; set; }
    public StateActorAuditDto? CurrentActor { get; set; }
  }
}
