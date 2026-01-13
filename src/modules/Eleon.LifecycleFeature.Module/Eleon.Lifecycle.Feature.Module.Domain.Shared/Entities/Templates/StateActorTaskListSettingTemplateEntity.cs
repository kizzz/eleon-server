using Common.Module.Constants;
using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Lifecycle.Feature.Module.Entities.Templates
{

  public class StateActorTaskListSettingTemplateEntity : Entity<Guid>, IHasCreationTime, IMultiTenant
  {
    public virtual Guid? TenantId { get; set; }
    public virtual DateTime CreationTime { get; set; }
    public virtual string DocumentObjectType { get; set; }
    public virtual Guid TaskListId { get; set; }
    public virtual Guid StateActorTemplateId { get; set; }
    public virtual StateActorTemplateEntity StateActorTemplate { get; set; }

    public StateActorTaskListSettingTemplateEntity()
        : base()
    {

    }
    public StateActorTaskListSettingTemplateEntity(Guid id)
        : base(id)
    {

    }
  }
}
