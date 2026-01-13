using Common.Module.Constants;
using System;

namespace VPortal.Lifecycle.Feature.Module.Dto.Templates
{
  public class StateActorTaskListSettingAuditDto
  {
    public string DocumentObjectType { get; set; }
    public Guid TaskListId { get; set; }
  }
}
