namespace Eleon.Logging.Lib.VportalLogging;

public interface IOperationScopeFactory
{
  IDisposable Begin(string operationName, IReadOnlyDictionary<string, object?>? context = null);
}
