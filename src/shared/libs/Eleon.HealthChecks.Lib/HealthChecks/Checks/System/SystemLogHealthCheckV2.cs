using Eleon.Logging.Lib.SystemLog.Logger;
using Eleon.Logging.Lib.SystemLog.Sinks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.System;

/// <summary>
/// System log health check implementing IHealthCheck.
/// Checks for queued log entries in sinks.
/// </summary>
public class SystemLogHealthCheckV2 : IHealthCheck
{
    private readonly ILogger<SystemLogHealthCheckV2> _logger;

    public SystemLogHealthCheckV2(ILogger<SystemLogHealthCheckV2> logger)
    {
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sinks = EleonsoftLog.GetSinksSnapshot() ?? Array.Empty<object>();

            int totalSinks = 0;
            int errors = 0;
            int totalFlushed = 0;
            var sinkResults = new List<Dictionary<string, object>>();

            foreach (var sink in sinks)
            {
                var sinkType = sink?.GetType().FullName ?? "(null)";

                try
                {
                    AbstractQueuedSystemLogSink? queuedSink = null;

                    if (sink is AbstractQueuedSystemLogSink qs1)
                        queuedSink = qs1;
                    else if (sink is FilteringSink fs && fs.Sink is AbstractQueuedSystemLogSink qs2)
                        queuedSink = qs2;

                    if (queuedSink != null)
                    {
                        totalSinks++;
                        var flushed = await queuedSink.TryFlushNowAsync();
                        var flushedCount = flushed.Key;
                        totalFlushed += flushedCount;

                        var sinkResult = new Dictionary<string, object>
                        {
                            ["sink_type"] = sinkType,
                            ["flushed_count"] = flushedCount,
                            ["has_failed_logs"] = flushed.Value.Length > 0
                        };

                        if (flushed.Value.Length > 0)
                        {
                            errors++;
                            sinkResult["failed_logs_count"] = flushed.Value.Length;
                        }

                        sinkResults.Add(sinkResult);
                    }
                }
                catch (Exception ex)
                {
                    errors++;
                    sinkResults.Add(new Dictionary<string, object>
                    {
                        ["sink_type"] = sinkType,
                        ["error"] = $"{ex.GetType().Name}: {ex.Message}"
                    });
                }
            }

            var data = new Dictionary<string, object>
            {
                ["log.sinks_total"] = totalSinks,
                ["log.total_flushed"] = totalFlushed,
                ["log.errors"] = errors
            };

            if (sinkResults.Any())
            {
                data["log.sinks"] = string.Join("; ", sinkResults.Select(s => 
                    $"{s.GetValueOrDefault("sink_type")}:{s.GetValueOrDefault("flushed_count", 0)}"));
            }

            if (errors > 0)
            {
                return HealthCheckResult.Unhealthy(
                    $"Errors occurred while checking log sinks: {errors} error(s)",
                    data: data);
            }

            if (totalFlushed > 0)
            {
                return HealthCheckResult.Degraded(
                    $"Queued log entries were found and flushed: {totalFlushed} entry(ies)",
                    data: data);
            }

            return HealthCheckResult.Healthy(
                "No queued log entries detected",
                data);
        }
        catch (OperationCanceledException)
        {
            return HealthCheckResult.Unhealthy("System log check cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in system log health check");
            return HealthCheckResult.Unhealthy($"Error: {ex.Message}");
        }
    }
}
