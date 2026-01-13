using Common.Module.Constants;
using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Lifecycle.Feature.Module.Entities
{
  public class StateTemplateEntity : Entity<Guid>, IHasCreationTime, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual DateTime CreationTime { get; set; }
    public virtual StatesGroupTemplateEntity StatesGroupTemplate { get; set; }
    public virtual Guid StatesGroupTemplateId { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual int OrderIndex { get; set; }
    public virtual string StateName { get; set; }
    public virtual bool IsMandatory { get; set; }
    public virtual bool IsReadOnly { get; set; }
    public virtual LifecycleApprovalType ApprovalType { get; set; }
    public virtual List<StateActorTemplateEntity> Actors { get; set; }

    public StateTemplateEntity()
        : base()
    {
      Actors = new List<StateActorTemplateEntity>();
    }
    public StateTemplateEntity(Guid id)
        : base(id)
    {
      Actors = new List<StateActorTemplateEntity>();
    }
  }
}
