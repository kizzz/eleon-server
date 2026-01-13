using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Lifecycle.Feature.Module.Entities
{
  public class StatesGroupTemplateEntity : FullAuditedAggregateRoot<Guid>, IHasCreationTime, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public new virtual DateTime CreationTime { get; set; }
    public virtual string DocumentObjectType { get; set; }
    public virtual string GroupName { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual List<StateTemplateEntity> States { get; set; }
    public StatesGroupTemplateEntity()
        : base()
    {
      States = new List<StateTemplateEntity>();
    }
    public StatesGroupTemplateEntity(Guid id)
        : base(id)
    {
      States = new List<StateTemplateEntity>();
    }
  }
}
