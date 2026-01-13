using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.LanguageManagement.Module.Entities
{
  public class LocalizationEntryEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public string CultureName { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public string ResourceName { get; set; }

    public LocalizationEntryEntity(Guid id)
    {
      Id = id;
    }
  }
}
