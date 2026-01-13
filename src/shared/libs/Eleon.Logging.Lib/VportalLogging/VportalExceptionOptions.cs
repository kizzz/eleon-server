namespace Eleon.Logging.Lib.VportalLogging;

public sealed class VportalExceptionOptions
{
  public bool CaptureBusinessExceptionsToSentry { get; set; } = false;
  public bool CaptureCancellationToSentry { get; set; } = false;

  public Func<Exception, bool>? SuppressPredicate { get; set; }

  public bool PreferRouteTemplateOverPath { get; set; } = true;
}
