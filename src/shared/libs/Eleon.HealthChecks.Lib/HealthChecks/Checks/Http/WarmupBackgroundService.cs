using EleonsoftSdk.modules.HealthCheck.Module.Checks.HttpCheck;
using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Http;
public class WarmupBackgroundService : BackgroundService
{
  private readonly IConfiguration _configuration;
  private readonly IServiceProvider _serviceProvider;
  private readonly IBoundaryLogger _boundaryLogger;

  public WarmupBackgroundService(
      IConfiguration configuration,
      IServiceProvider serviceProvider,
      IBoundaryLogger boundaryLogger)
  {
    _configuration = configuration;
    _serviceProvider = serviceProvider;
    _boundaryLogger = boundaryLogger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    using var _ = _boundaryLogger.Begin("HostedService WarmupBackgroundService");
    var warmupMinutes = _configuration.GetValue<int>("WarmupEveryMinutes", 10);

    if (warmupMinutes > 0)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          await Task.Delay(TimeSpan.FromMinutes(warmupMinutes), stoppingToken).ConfigureAwait(false);
          using var scope = _serviceProvider.CreateScope();
          var httpCheck = scope.ServiceProvider.GetRequiredService<HttpHealthCheck>();
          await httpCheck.CheckAsync(Guid.Empty).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
          // Ignore cancellation exceptions
        }
      }
    }
  }
}

public static class WarmupBackgroundServiceExtensions
{
  public static IServiceCollection AddWarmupBackgroundService(this IServiceCollection services)
  {
    services.AddTransient<HttpHealthCheck>();
    services.AddHostedService<WarmupBackgroundService>();
    return services;
  }
}
