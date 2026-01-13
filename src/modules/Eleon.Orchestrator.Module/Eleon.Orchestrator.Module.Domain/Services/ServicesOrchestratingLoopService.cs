
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Eleon.Logging.Lib.VportalLogging;
using ServicesOrchestrator.Services.Abstractions;
using Volo.Abp.DependencyInjection;

namespace ServicesOrchestrator.Services;


public class ServicesOrchestratingLoopService : BackgroundService, ITransientDependency
{
  private readonly IOrchestratingService _orchestrator;
  private readonly IOrchestratingConfigurationService _config;
  private readonly ILogger<ServicesOrchestratingLoopService> _log;
  private readonly IBoundaryLogger _boundaryLogger;

  public ServicesOrchestratingLoopService(
      IOrchestratingService orchestrator,
      IOrchestratingConfigurationService config,
      ILogger<ServicesOrchestratingLoopService> log,
      IBoundaryLogger boundaryLogger)
  {
    _orchestrator = orchestrator;
    _config = config;
    _log = log;
    _boundaryLogger = boundaryLogger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    using var _ = _boundaryLogger.Begin("HostedService ServicesOrchestratingLoopService");
    var opts = _config.Options;

    if (!opts.Enabled)
    {
      return;
    }

    // Ensure graceful flush on exit as well
    AppDomain.CurrentDomain.ProcessExit += (_, __) => { _orchestrator.StopAsync(default).Wait(5000); _orchestrator.ProcessAsync(default).Wait(25000); };

    _log.LogDebug("Orchestrating loop starting...");
    await _orchestrator.StartAsync(stoppingToken); // enable orchestration

    while (!stoppingToken.IsCancellationRequested)
    {
      int delayMs = opts.DefaultTickMs;

      try
      {
        delayMs = await _orchestrator.ProcessAsync(stoppingToken);
        if (delayMs <= 0) delayMs = opts.DefaultTickMs;
        delayMs = Math.Clamp(delayMs, opts.MinTickMs, opts.MaxTickMs);
      }
      catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
      {
        break;
      }
      catch (Exception ex)
      {
        _log.LogError(ex, "Orchestrator tick failed");
      }

      try { await Task.Delay(delayMs, stoppingToken); }
      catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
    }

    _log.LogDebug("Orchestrating loop stopping...");
    // Ensure all services are stopped when the host is shutting down
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    try { await _orchestrator.StopAsync(cts.Token); }
    catch (Exception ex) { _log.LogWarning(ex, "StopAsync threw during host shutdown."); }

    _log.LogDebug("Orchestrating loop stopped.");
  }
}
