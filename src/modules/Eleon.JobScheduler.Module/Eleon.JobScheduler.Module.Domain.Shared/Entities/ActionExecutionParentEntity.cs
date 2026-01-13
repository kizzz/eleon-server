using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.JobScheduler.Module.Entities
{
  [DisableAuditing]
  public class ActionExecutionParentEntity : Entity, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual Guid ChildActionExecutionId { get; set; }
    public virtual Guid ParentActionExecutionId { get; set; }

    protected ActionExecutionParentEntity() { }

    public ActionExecutionParentEntity(Guid childId, Guid parentId)
    {
      this.ChildActionExecutionId = childId;
      this.ParentActionExecutionId = parentId;
    }

    public override object[] GetKeys()
    {
      return new object[] { ChildActionExecutionId, ParentActionExecutionId };
    }
  }
}
