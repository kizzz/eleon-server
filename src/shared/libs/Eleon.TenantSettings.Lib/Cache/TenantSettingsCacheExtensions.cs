namespace TenantSettings.Module.Cache
{
  public static class TenantSettingsCacheLinqExtensions
  {
    public static TenantSettingDictionary<TSetting> ToTenantSettingsDictionary<TSource, TSetting>(
        this IEnumerable<TSource> seq,
        Func<TSource, string> keySelector,
        Func<TSource, TSetting> valueSelector,
        TSetting @default = default)
        => new TenantSettingDictionary<TSetting>(seq.ToDictionary(keySelector, valueSelector), @default);

    public static UserSettingDictionary<TSetting> ToUserSettingsDictionary<TSource, TSetting>(
        this IEnumerable<TSource> seq,
        Func<TSource, string> keySelector,
        Func<TSource, TSetting> valueSelector,
        TSetting @default = default)
        => new UserSettingDictionary<TSetting>(seq.ToDictionary(keySelector, valueSelector), @default);
  }
}
