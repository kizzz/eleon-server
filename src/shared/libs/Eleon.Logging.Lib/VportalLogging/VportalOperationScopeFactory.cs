using Microsoft.Extensions.Logging;

namespace Eleon.Logging.Lib.VportalLogging;

public sealed class VportalOperationScopeFactory : IOperationScopeFactory
{
  private readonly ILogger<VportalOperationScopeFactory> _logger;

  public VportalOperationScopeFactory(ILogger<VportalOperationScopeFactory> logger)
  {
    _logger = logger;
  }

  public IDisposable Begin(string operationName, IReadOnlyDictionary<string, object?>? context = null)
  {
    var scope = new Dictionary<string, object?>(StringComparer.Ordinal)
    {
      [VportalLogProperties.Operation] = operationName
    };

    if (context != null)
    {
      foreach (var entry in context)
      {
        if (!scope.ContainsKey(entry.Key))
        {
          scope[entry.Key] = entry.Value;
        }
      }
    }

    return _logger.BeginScope(scope);
  }
}
