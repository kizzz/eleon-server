using EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Runtime;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Environment;

/// <summary>
/// Current process health check implementing IHealthCheck.
/// Monitors current process CPU, memory, threads, and GC.
/// </summary>
public class CurrentProcessHealthCheckV2 : IHealthCheck
{
    private readonly CurrentProcessHealthCheckOptions _options;
    private readonly ILogger<CurrentProcessHealthCheckV2> _logger;

    public CurrentProcessHealthCheckV2(
        IOptions<CurrentProcessHealthCheckOptions> options,
        ILogger<CurrentProcessHealthCheckV2> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var proc = Process.GetCurrentProcess();
            
            // CPU sampling
            var cpu = await SampleCurrentProcessCpuAsync(proc, _options.CpuSampleMilliseconds, cancellationToken);
            var cpuFail = cpu > _options.CpuThresholdPercent;

            // Memory
            proc.Refresh();
            double wsMB = BytesToMB(Safe(() => proc.WorkingSet64));
            double privateMB = BytesToMB(Safe(() => proc.PrivateMemorySize64));
            var gci = GC.GetGCMemoryInfo();
            double heapMB = BytesToMB(gci.HeapSizeBytes);
            var memFail = wsMB > _options.WorkingSetThresholdMB || heapMB > _options.HeapThresholdMB;

            // Threads
            var threadCount = Safe(() => proc.Threads?.Count) ?? 0;
            var threadFail = threadCount > _options.ThreadThreshold;

            // ThreadPool
            ThreadPool.GetAvailableThreads(out var availWorker, out var availIO);
            ThreadPool.GetMaxThreads(out var maxWorker, out var maxIO);
            var busyWorker = maxWorker - availWorker;

            var data = new Dictionary<string, object>
            {
                ["proc.cpu_percent"] = Math.Round(cpu, 2),
                ["proc.cpu_threshold"] = _options.CpuThresholdPercent,
                ["proc.working_set_mb"] = Math.Round(wsMB, 2),
                ["proc.working_set_threshold_mb"] = _options.WorkingSetThresholdMB,
                ["proc.heap_mb"] = Math.Round(heapMB, 2),
                ["proc.heap_threshold_mb"] = _options.HeapThresholdMB,
                ["proc.threads"] = threadCount,
                ["proc.thread_threshold"] = _options.ThreadThreshold,
                ["proc.threadpool_available_worker"] = availWorker,
                ["proc.threadpool_busy_worker"] = busyWorker,
                ["proc.gc_gen0"] = GC.CollectionCount(0),
                ["proc.gc_gen1"] = GC.CollectionCount(1),
                ["proc.gc_gen2"] = GC.CollectionCount(2),
                ["proc.gc_latency_mode"] = GCSettings.LatencyMode.ToString()
            };

            var failedReasons = new List<string>();
            if (cpuFail) failedReasons.Add($"CPU>{_options.CpuThresholdPercent}%");
            if (memFail) failedReasons.Add($"Memory thresholds exceeded");
            if (threadFail) failedReasons.Add($"Threads>{_options.ThreadThreshold}");

            if (failedReasons.Any())
            {
                return HealthCheckResult.Unhealthy(
                    $"Current process limits exceeded: {string.Join(", ", failedReasons)}",
                    data: data);
            }

            return HealthCheckResult.Healthy(
                "Current process is within thresholds",
                data);
        }
        catch (OperationCanceledException)
        {
            return HealthCheckResult.Unhealthy("Current process check cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in current process health check");
            return HealthCheckResult.Unhealthy($"Error: {ex.Message}");
        }
    }

    private static double BytesToMB(long bytes) => Math.Round(bytes / 1024.0 / 1024.0, 2);
    private static T? Safe<T>(Func<T> fn) { try { return fn(); } catch { return default; } }

    private static async Task<double> SampleCurrentProcessCpuAsync(Process p, int sampleMs, CancellationToken ct)
    {
        var cores = Math.Max(1, global::System.Environment.ProcessorCount);
        var t1 = Safe(() => p.TotalProcessorTime.TotalMilliseconds);
        var sw = Stopwatch.StartNew();
        await Task.Delay(sampleMs, ct);
        var elapsed = Math.Max(1, sw.ElapsedMilliseconds);
        var t2 = Safe(() => p.TotalProcessorTime.TotalMilliseconds);

        var delta = Math.Max(0.0, t2 - t1);
        var cpu = (delta / (elapsed * cores)) * 100.0;
        return Math.Min(100.0, Math.Max(0.0, cpu));
    }
}
