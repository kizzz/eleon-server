namespace Eleon.Logging.Lib.VportalLogging;

public sealed class VportalLoggingHostOptions
{
  public bool EnableHttpRequestLogging { get; set; } = true;
  public bool EnableSignalRLogging { get; set; } = true;
  public bool EnableCompatVportalLogger { get; set; } = true;
}
