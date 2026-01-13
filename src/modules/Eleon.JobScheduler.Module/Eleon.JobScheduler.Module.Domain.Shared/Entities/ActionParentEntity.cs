using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.JobScheduler.Module.Entities
{
  [Audited]
  public class ActionParentEntity : Entity, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual Guid ChildActionId { get; set; }
    public virtual Guid ParentActionId { get; set; }

    protected ActionParentEntity() { }

    public ActionParentEntity(Guid childId, Guid parentId)
    {
      this.ChildActionId = childId;
      this.ParentActionId = parentId;
    }

    public override object[] GetKeys()
    {
      return new object[] { ChildActionId, ParentActionId };
    }
  }
}
