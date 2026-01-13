using TenantSettings.Module.Models;

namespace TenantSettings.Module.Messaging
{
  public class TenantSettingsCacheEto
  {
    public List<TenantSetting> TenantSettings { get; set; }
    public List<UserIsolationSettings> UserIsolationSettings { get; set; }
    public List<Guid> HostAdminUsers { get; set; }
    public string[] Cors { get; set; }
  }
}
