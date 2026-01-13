using EleonsoftModuleCollector.Commons.Module.Constants.IdentitySettings;

namespace VPortal.TenantManagement.Module.Settings
{
  public class IdentitySettingDto
  {
    public string Name { get; set; }
    public string GroupName { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public IdentitySettingType Type { get; set; }
    public string Value { get; set; }
  }
}
