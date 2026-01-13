using EleonsoftSdk.modules.HealthCheck.Module.Base;
using Eleon.Logging.Lib.VportalLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedModule.modules.Helpers.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EleonsoftSdk.modules.HealthCheck.Module.General.BackgroundExecution;

public class OnStartHealthCheckService : BackgroundService
{
  private readonly ILogger<OnStartHealthCheckService> _logger;
  private readonly IHostApplicationLifetime _lifetime;
  private readonly IServiceProvider _serviceProvider;
  private readonly IBoundaryLogger _boundaryLogger;

  public OnStartHealthCheckService(
      ILogger<OnStartHealthCheckService> logger,
      IHostApplicationLifetime lifetime,
      IServiceProvider serviceProvider,
      IBoundaryLogger boundaryLogger)
  {
    _logger = logger;
    _lifetime = lifetime;
    _serviceProvider = serviceProvider;
    _boundaryLogger = boundaryLogger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    using var boundaryScope = _boundaryLogger.Begin("HostedService OnStartHealthCheckService");
    _lifetime.ApplicationStarted.Register(() =>
    {
      _ = Task.Run(async () =>
          {
          await AddStartupHealthCheck();
        });
    });
    
    // Keep service alive
    await Task.Delay(Timeout.Infinite, stoppingToken);
  }

  private async Task AddStartupHealthCheck()
  {
    _logger.LogDebug("Starting health check execution...");
    try
    {
      using var scope = _serviceProvider.CreateScope();
      var _options = scope.ServiceProvider.GetRequiredService<IOptions<HealthCheckOptions>>().Value;

      // Try new coordinator first, fall back to old manager for backward compatibility
      var coordinator = scope.ServiceProvider.GetService<EleonsoftSdk.modules.HealthCheck.Module.Core.IHealthRunCoordinator>();
      if (coordinator != null)
      {
        var startType = StartupDiagnostics.DetectStartupType();
        await coordinator.RunAsync(
            type: startType,
            initiatorName: _options.ApplicationName,
            options: null,
            ct: default)
            .ConfigureAwait(false);
      }
      else
      {
        // Fallback to old manager
        var _manager = scope.ServiceProvider.GetRequiredService<HealthCheckManager>();
        var startType = StartupDiagnostics.DetectStartupType();
        await _manager
            .ExecuteHealthCheckAsync(startType, _options.ApplicationName)
            .ConfigureAwait(false);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Execute health check failed");
    }
  }
}
