using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace ServicesOrchestrator.Services.Abstractions;

public interface IOrchestratingService
{
  /// Runs one reconciliation tick and returns milliseconds until next tick.
  Task<int> ProcessAsync(CancellationToken ct);

  /// Enables orchestration (subsequent ProcessAsync will act).
  Task StartAsync(CancellationToken ct);

  /// Disables orchestration and stops all managed services.
  Task StopAsync(CancellationToken ct);

  Task<OrchestratorStatusDto> GetStatusAsync(CancellationToken ct);
}
