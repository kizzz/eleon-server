

namespace HealthCheckModule.Module.Domain.Shared.Constants;
public static class HealthCheckDefaults
{
  public static class HealthCheckTypes
  {
    public const string Manual = "Manual";
    public const string Scheduled = "Scheduled";
    public const string OnStart = "OnStart";
    public const string Upgrate = "Upgrade";
  }

  public static class Inititors
  {
    public const string System = "System";
    public const string Unauthorized = "Unauthorized";
  }

  public static class ExtraInfoTypes
  {
    public const string Simple = "simple";
    public const string Json = "json";
  }
}
