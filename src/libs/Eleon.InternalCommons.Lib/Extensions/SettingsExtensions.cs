using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.SettingManagement;

namespace EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Settings;
public static class SettingsExtensions
{
  private readonly static JsonSerializerOptions _options = new JsonSerializerOptions
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = false,
  };

  public static async Task<T> GetOrDefaultForCurrentTenantAsync<T>(this ISettingManager settingManager, string key, T defaultValue = null) where T : class, new()
  {
    var str = await settingManager.GetOrNullForCurrentTenantAsync(key);
    if (!string.IsNullOrWhiteSpace(str))
    {
      try
      {
        var obj = JsonSerializer.Deserialize<T>(str, _options);
        if (obj != null)
        {
          return obj;
        }
      }
      catch (Exception)
      {
        // Ignore deserialization errors and return default value
      }
    }
    return defaultValue ?? new T();
  }

  public static async Task SetForCurrentTenantAsync<T>(this ISettingManager settingManager, string key, T value) where T : class, new()
  {
    var str = JsonSerializer.Serialize(value, _options);
    await settingManager.SetForCurrentTenantAsync(key, str);
  }

  public static async Task<T> GetOrDefaultForCurrentUserAsync<T>(this ISettingManager settingManager, string key, T defaultValue = null) where T : class, new()
  {
    var str = await settingManager.GetOrNullForCurrentUserAsync(key);
    if (!string.IsNullOrWhiteSpace(str))
    {
      try
      {
        var obj = JsonSerializer.Deserialize<T>(str, _options);
        if (obj != null)
        {
          return obj;
        }
      }
      catch (Exception)
      {
        // Ignore deserialization errors and return default value
      }
    }
    return defaultValue ?? new T();
  }

  public static async Task SetForCurrentUserAsync<T>(this ISettingManager settingManager, string key, T value) where T : class, new()
  {
    var str = JsonSerializer.Serialize(value, _options);
    await settingManager.SetForCurrentUserAsync(key, str);
  }
}
