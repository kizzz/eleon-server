using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using VPortal.Lifecycle.Feature.Module.Entities.Templates;

namespace VPortal.Lifecycle.Feature.Module.Entities
{
  public class StateActorTemplateEntity : Entity<Guid>, IHasCreationTime, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual DateTime CreationTime { get; set; }
    [NotMapped]
    public virtual string DisplayName { get; set; }
    public virtual string ActorName { get; set; }
    public virtual StateTemplateEntity StateTemplateEntity { get; set; }
    public virtual Guid StateTemplateId { get; set; }
    public virtual int? OrderIndex { get; set; }
    public virtual string RefId { get; set; }
    public virtual LifecycleActorTypes ActorType { get; set; }
    public virtual bool IsConditional { get; set; }
    public virtual Guid RuleId { get; set; }
    public virtual bool IsApprovalNeeded { get; set; }
    public virtual bool IsFormAdmin { get; set; }
    public virtual bool IsApprovalManager { get; set; }
    public virtual bool IsApprovalAdmin { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual List<StateActorTaskListSettingTemplateEntity> TaskLists { get; set; }

    public StateActorTemplateEntity()
        : base()
    {
      TaskLists = new List<StateActorTaskListSettingTemplateEntity>();
    }
    public StateActorTemplateEntity(Guid id)
        : base(id)
    {
      TaskLists = new List<StateActorTaskListSettingTemplateEntity>();
    }
  }
}
