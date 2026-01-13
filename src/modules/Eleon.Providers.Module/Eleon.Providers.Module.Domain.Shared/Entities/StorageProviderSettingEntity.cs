using Eleon.Storage.Lib.Constants;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Entities;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Storage.Module.Entities
{
  public class StorageProviderSettingEntity : Entity<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public Guid StorageProviderId { get; set; }
    public string Value { get; set; }
    public string Key { get; set; }

    protected StorageProviderSettingEntity() { }

    public StorageProviderSettingEntity(Guid id)
        : base(id)
    {
    }
  }
}
