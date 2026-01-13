namespace TenantSettings.Module.Cache
{
  public class SettingDictionary<T>
  {
    protected readonly Dictionary<string, T> dict;
    protected readonly T @default;

    public SettingDictionary(Dictionary<string, T> dict, T @default)
    {
      this.dict = dict;
      this.@default = @default;
    }
  }

  public class TenantSettingDictionary<T> : SettingDictionary<T>
  {
    public TenantSettingDictionary(Dictionary<string, T> dict, T @default)
        : base(dict, @default)
    {
    }

    public T this[Guid? tenantId]
    {
      get => dict.GetValueOrDefault(TenantSettingsCacheKeys.GetTenantKey(tenantId), @default);
    }
  }

  public class UserSettingDictionary<T> : SettingDictionary<T>
  {
    public UserSettingDictionary(Dictionary<string, T> dict, T @default)
        : base(dict, @default)
    {
    }

    public T this[Guid? tenantId, Guid userId]
    {
      get => dict.GetValueOrDefault(TenantSettingsCacheKeys.GetUserKey(tenantId, userId), @default);
    }
  }
}
