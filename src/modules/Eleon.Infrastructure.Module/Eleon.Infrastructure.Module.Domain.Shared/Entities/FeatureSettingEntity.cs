using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Core.Infrastructure.Module.Entities
{
  public class FeatureSettingEntity : AggregateRoot<Guid>, IHasCreationTime, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public virtual DateTime CreationTime { get; set; }
    public virtual string Group { get; set; }
    public virtual string Key { get; set; }
    public virtual string Value { get; set; }
    public virtual string Type { get; set; }
    public virtual bool IsEncrypted { get; set; }
    public virtual bool IsRequired { get; set; }

    public FeatureSettingEntity() { }

    public FeatureSettingEntity(
        [NotNull] string group,
        [NotNull] string key,
        [NotNull] string val,
        [NotNull] string type,
        [NotNull] bool isEncrypted,
        [NotNull] bool isRequired,
        Guid? tenantId)
    {
      Group = group;
      Key = key;
      Value = val;
      Type = type;
      IsEncrypted = isEncrypted;
      IsRequired = isRequired;
      TenantId = tenantId;
      CreationTime = DateTime.Now;
    }
  }
}
