namespace Eleon.Logging.Lib.VportalLogging;

public sealed class VportalRequestLoggingOptions
{
  public bool Enable { get; set; } = true;
  public bool PreferRouteTemplateOverPath { get; set; } = true;

  public List<string> ExcludedPathPrefixes { get; set; } = new()
  {
    "/health",
    "/metrics",
    "/swagger"
  };
}
