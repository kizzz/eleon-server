using Common.Module.Constants;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Lifecycle.Feature.Module.Entities.Conditions
{
  public class RuleEntity : Entity<Guid>, IMultiTenant
  {
    public DocumentTemplateElementMapFunctionType FunctionType { get; set; }
    public string Function { get; set; }
    public bool IsEnabled { get; set; }
    public Guid? TenantId { get; set; }
    protected RuleEntity() { }
    public RuleEntity(Guid id) : base(id) { }
  }
}
