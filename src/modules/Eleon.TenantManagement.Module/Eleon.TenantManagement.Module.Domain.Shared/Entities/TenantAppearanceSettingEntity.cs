using System;
using Volo.Abp.Domain.Entities;

namespace VPortal.TenantManagement.Module.Entities
{
  public class TenantAppearanceSettingEntity : Entity<Guid>
  {
    public string LightLogo { get; set; } = string.Empty;
    public string LightIcon { get; set; } = string.Empty;
    public string DarkLogo { get; set; } = string.Empty;
    public string DarkIcon { get; set; } = string.Empty;

    protected TenantAppearanceSettingEntity() { }

    public TenantAppearanceSettingEntity(Guid id)
    {
      Id = id;
    }
  }
}
