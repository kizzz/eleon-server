using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.Storage.Module.Entities
{
  public class StorageProviderEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }

    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsTested { get; set; }
    public string StorageProviderTypeName { get; set; }

    public IList<StorageProviderSettingEntity> Settings { get; set; }

    [NotMapped]
    public string FullType { get; set; }

    protected StorageProviderEntity() { }

    public StorageProviderEntity(Guid id)
        : base(id)
    {
    }

    public Dictionary<string, string> GetSettingsDictionary()
    {
      if (Settings == null) return null;
      Dictionary<string, string> res = new();
      foreach (var setting in Settings)
      {
        res[setting.Key] = setting.Value;
      }

      return res;
    }
  }
}
