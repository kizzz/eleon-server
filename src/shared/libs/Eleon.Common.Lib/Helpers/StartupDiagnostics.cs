using Eleon.Logging.Lib.SystemLog.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedModule.modules.Helpers.Module;
public static class StartupDiagnostics
{
  public const string OnStart = "OnStart";
  public const string Recycle = "Recycle";
  public const string Reboot = "Reboot";
  public const string Upgrade = "Upgrade";

  private const string UPGRADED_KEY = "UPGRADED";

  private static bool CheckUpdatedBaseOnAppsettings()
  {
    // Get appsettings.json path relative to the exe
    var exeDir = AppContext.BaseDirectory;
    var settingsFile = Path.Combine(exeDir, "appsettings.json");

    if (!File.Exists(settingsFile))
    {
      // No settings file -> treat as first upgrade
      return true;
    }

    // Load JSON as a modifiable dictionary
    var json = File.ReadAllText(settingsFile);
    using var doc = JsonDocument.Parse(json);
    var root = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

    if (root == null)
    {
      return true;
    }

    if (root.TryGetValue(UPGRADED_KEY, out var value) && value is JsonElement elem && elem.ValueKind == JsonValueKind.True)
    {
      return false;
    }

    // Mark it as true
    root[UPGRADED_KEY] = true;

    var newJson = JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(settingsFile, newJson);
    return true;
  }

  private static bool _isInitialized = false;
  private static bool _isStartupAfterUpdate = false;
  private static bool IsStartupAfterUpgrade
  {
    get
    {
      try
      {
        if (!_isInitialized)
        {
          _isInitialized = true;
          // _isStartupAfterUpdate = CheckUpdatedBaseOnAppsettings();
        }
      }
      catch (Exception ex)
      {
        EleonsoftLog.Error("Failed to determine if startup is after upgrade", ex);
      }

      return _isStartupAfterUpdate;
    }
  }

  private static string _cachedStartupType;
  private static string PrivateDetectStartupType()
  {
    if (IsStartupAfterUpgrade)
      return Upgrade;

    var process = Process.GetCurrentProcess();
    var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);

    if (uptime.TotalMinutes < 5)
      return Reboot;
    if ((DateTime.Now - process.StartTime).TotalMinutes < 2)
      return Recycle;
    return OnStart;
  }
  public static string DetectStartupType()
  {
    if (string.IsNullOrWhiteSpace(_cachedStartupType))
    {
      _cachedStartupType = PrivateDetectStartupType();
    }

    return _cachedStartupType;
  }

  public static TimeSpan GetUptime()
  {
    var process = Process.GetCurrentProcess();
    return DateTime.Now - process.StartTime;
  }

  public static string GetPrettyStartupType()
  {
    var startType = DetectStartupType();
    return startType switch
    {
      OnStart => "✅ Start",
      Recycle => "🔁 App Pool Recycle",
      Reboot => "🖥️ Server Reboot",
      Upgrade => "⬆️ Application Upgrade",
      _ => "Unknown"
    };
  }
}
