using System;
using System.Linq;

namespace VPortal
{
  public static class StartupArgsParser
  {
    private static readonly Dictionary<string, string> Cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private static string[] Args { get; set; } = [];

    public static void SetArgs(string[] args)
    {
      Cache.Clear();
      Args = args ?? [];
    }

    public static void Set(string key, string value)
    {
      Cache[key] = value;
    }

    public static bool IsRun => GetBoolArg("run");
    public static bool IsMigrate => GetBoolArg("migrate");
    public static bool IsSeed => GetBoolArg("seed");

    public static string LogsPath => GetArgValue("logs-path");
    public static string ErrorLogsPath => GetArgValue("error-logs-path");
    public static string LogsFilePrefix => GetArgValue("logs-file-prefix");

    public static string ConfigurationProfile => GetArgValue("config");

    public static bool GetBoolArg(string key)
    {
      var value = GetArgValue(key);
      return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetArgValue(string key)
    {
      if (Cache.TryGetValue(key, out var cachedValue))
      {
        return cachedValue;
      }

      var prefix = $"--{key}=";
      var arg = Args.FirstOrDefault(a => a.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
      if (arg == null)
      {
        Cache[key] = string.Empty;
        return string.Empty;
      }

      var value = arg.Substring(prefix.Length).Trim('"');
      value = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
      Cache[key] = value;
      return value;
    }

    public static string[] Raw => Args;
  }
}
