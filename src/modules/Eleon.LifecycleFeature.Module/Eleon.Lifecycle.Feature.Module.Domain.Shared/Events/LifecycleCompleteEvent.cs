using Common.Module.Constants;
using System;
using VPortal.Lifecycle.Feature.Module.Entities;

namespace VPortal.Lifecycle.Feature.Module.Events
{
  public class LifecycleCompleteEvent
  {
    public string ObjectType { get; set; }
    public Guid DocId { get; set; }
    public StatesGroupAuditEntity Audit { get; set; }
    public Guid? TenantId { get; set; }
  }
}
