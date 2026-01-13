using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Lifecycle.Feature.Module.Entities.Conditions
{
  public class ConditionEntity : AggregateRoot<Guid>, IMultiTenant
  {
    public LifecycleConditionTargetType ConditionTargetType { get; set; }
    public LifecycleConditionType ConditionType { get; set; }
    public LifecycleConditionResultType ConditionResultType { get; set; }
    public Guid RefId { get; set; }
    public virtual List<RuleEntity> Rules { get; set; }
    public bool IsEnabled { get; set; }
    public Guid? TenantId { get; set; }
    protected ConditionEntity()
    {
    }

    public ConditionEntity(Guid id) : base(id)
    {
    }
  }
}
