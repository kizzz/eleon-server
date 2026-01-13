using TenantSettings.Module.Models;

namespace TenantSettings.Module.Cache
{
  internal class TenantSettingsCacheKeys
  {
    public static string GetUserKey(Guid? tenantId, Guid userId)
        => $"{GetTenantKey(tenantId)};{userId}";

    public static string GetUserKey(UserIsolationSettings settings)
        => GetUserKey(settings.TenantId, settings.UserId);

    public static string GetTenantKey(Guid? tenantId)
        => tenantId.ToString().IsNullOrEmpty() ? "host" : tenantId.ToString();

    public static string GetTenantKey(TenantSetting tenantSetting)
        => GetTenantKey(tenantSetting.TenantId);
  }
}
