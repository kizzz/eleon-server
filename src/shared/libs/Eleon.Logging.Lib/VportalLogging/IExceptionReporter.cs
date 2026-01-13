namespace Eleon.Logging.Lib.VportalLogging;

public interface IExceptionReporter
{
  void Report(Exception ex, IReadOnlyDictionary<string, object?>? context = null);
  bool ShouldSuppress(Exception ex);
}
