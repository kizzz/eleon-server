using Eleon.Storage.Lib.Constants;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Entities;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.Storage.Module.Entities
{
  public class StorageProviderSettingTypeEntity : Entity<Guid>
  {
    public string StorageProviderTypeName { get; set; }
    public StorageProviderSettingsTypes Type { get; set; }
    public string Key { get; set; }
    public string DefaultValue { get; set; }
    public string Description { get; set; }
    public bool Hidden { get; set; }
    public bool Required { get; set; }

    protected StorageProviderSettingTypeEntity() { }

    public StorageProviderSettingTypeEntity(Guid id)
        : base(id)
    {
    }
  }
}
