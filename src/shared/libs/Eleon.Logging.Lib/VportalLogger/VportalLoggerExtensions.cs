using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.DependencyInjection;

namespace Eleon.Logging.Lib.VportalLogger;

public static class VportalLoggerExtensions
{
  [Obsolete("Use AddVportalLogging() instead.")]
  public static IServiceCollection AddVportalLogger(this IServiceCollection services, Func<IServiceProvider, Guid?>? tenantIdAccessor = null)
  {
    _ = tenantIdAccessor;
    return services.AddVportalLogging();
  }
}
