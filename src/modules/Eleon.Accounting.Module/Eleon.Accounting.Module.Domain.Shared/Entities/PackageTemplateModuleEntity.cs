using Common.Module.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Accounting.Module.Entities
{
  [Audited]
  public class PackageTemplateModuleEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public virtual string Name { get; set; }
    public virtual Guid? TenantId { get; set; }
    public virtual Guid PackageTemplateEntityId { get; set; }
    public virtual Guid? RefId { get; set; }
    public virtual PackageModuleType ModuleType { get; set; }
    public virtual string Description { get; set; }
    public virtual string ModuleData { get; set; }

    public PackageTemplateModuleEntity() { }

    public PackageTemplateModuleEntity(Guid id)
        : base(id) { }
  }
}
