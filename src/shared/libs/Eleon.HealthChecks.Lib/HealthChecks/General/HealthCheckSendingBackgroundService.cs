using EleonsoftSdk.modules.HealthCheck.Module.Base;
using Eleon.Logging.Lib.VportalLogging;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.HealthCheck.Module.General;
public class HealthCheckSendingBackgroundService : BackgroundService
{
  private readonly HealthCheckManager? _healthCheckManager; // Nullable for backward compatibility
  private readonly HealthCheckOptions _options;
  private readonly IBoundaryLogger _boundaryLogger;
  private CancellationTokenSource? _cts = null;
  private DateTime _lastSentTime = DateTime.MinValue;

  public HealthCheckSendingBackgroundService(
      HealthCheckManager? healthCheckManager,
      IOptions<HealthCheckOptions> options,
      IBoundaryLogger boundaryLogger)
  {
    _healthCheckManager = healthCheckManager;
    _options = options.Value;
    _boundaryLogger = boundaryLogger;
  }

  public async Task TrySendAsync()
  {
    if (_cts != null)
    {
      if (_lastSentTime.AddHours(1) > DateTime.UtcNow)
      {
        _cts.Cancel();
      }
      else
      {
        return;
      }
    }

    _lastSentTime = DateTime.UtcNow;
    _cts = new CancellationTokenSource();

    try
    {
      if (_healthCheckManager != null)
      {
        await _healthCheckManager.SendHealthCheckAsync(_cts.Token);
      }
      // Note: New publishing is handled by HealthPublishingService
    }
    finally
    {
      _cts = null;
    }
  }

  protected async override Task ExecuteAsync(CancellationToken stoppingToken)
  {
    using var _ = _boundaryLogger.Begin("HostedService HealthCheckSendingBackgroundService");
    // Only run if old manager exists (backward compatibility)
    if (_healthCheckManager == null)
    {
      // New architecture uses HealthPublishingService instead
      await Task.Delay(Timeout.Infinite, stoppingToken);
      return;
    }

    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        await Task.Delay(TimeSpan.FromMinutes(_options.SendIntervalMinutes), stoppingToken);
        await TrySendAsync();
      }
      catch (OperationCanceledException) { /* ignore */ }
      catch (Exception) { /* ignore */ }
    }
  }
}
