using EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Environment;

/// <summary>
/// Environment health check implementing IHealthCheck.
/// Monitors CPU, memory, and disk usage.
/// </summary>
public class EnvironmentHealthCheckV2 : IHealthCheck
{
    private readonly EnvironmentHealthCheckOptions _options;
    private readonly ILogger<EnvironmentHealthCheckV2> _logger;

    public EnvironmentHealthCheckV2(
        IOptions<EnvironmentHealthCheckOptions> options,
        ILogger<EnvironmentHealthCheckV2> logger)
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

            // CPU sampling
            var cpuInfo = await SampleCpuAsync(_options.CpuSampleMilliseconds, cancellationToken);

            // Memory
            var totalAvailableBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            var processes = SafeGetProcesses();
            long totalWorkingSet = processes.Sum(p => SafeWS(p));
            double memPercent = totalAvailableBytes > 0
                ? (totalWorkingSet * 100.0 / totalAvailableBytes)
                : 0.0;

            // Disk
            var drives = DriveInfo.GetDrives()
                .Where(d => { try { return d.IsReady && d.DriveType != DriveType.CDRom; } catch { return false; } })
                .Select(d =>
                {
                    double usedPct = (d.TotalSize > 0)
                        ? (100.0 * (d.TotalSize - d.AvailableFreeSpace) / d.TotalSize)
                        : 0.0;
                    return new { d.Name, UsedPercent = Math.Round(usedPct, 2), TotalGB = SafeToGB(d.TotalSize), FreeGB = SafeToGB(d.AvailableFreeSpace) };
                })
                .OrderByDescending(x => x.UsedPercent)
                .ToList();

            // Threshold checks
            bool failCpu = cpuInfo.TotalCpuPercent > _options.CpuThresholdPercent;
            bool failMem = memPercent > _options.MemoryThresholdPercent;
            bool failDisk = _options.FailOnDiskThreshold && drives.Any(d => d.UsedPercent > _options.DiskThresholdPercent);

            var data = new Dictionary<string, object>
            {
                ["env.cpu_percent"] = Math.Round(cpuInfo.TotalCpuPercent, 2),
                ["env.cpu_threshold"] = _options.CpuThresholdPercent,
                ["env.memory_percent"] = Math.Round(memPercent, 2),
                ["env.memory_threshold"] = _options.MemoryThresholdPercent,
                ["env.total_memory_gb"] = SafeToGB(totalAvailableBytes),
                ["env.working_set_gb"] = SafeToGB(totalWorkingSet),
                ["env.disk_threshold"] = _options.DiskThresholdPercent,
                ["env.drives_count"] = drives.Count
            };

            // Add top CPU processes
            var topCpu = cpuInfo.PerProcess
                .OrderByDescending(p => p.CpuPercent)
                .Take(5)
                .Select(p => $"{p.Name}:{Math.Round(p.CpuPercent, 2)}%")
                .ToList();
            if (topCpu.Any())
                data["env.top_cpu_processes"] = string.Join(", ", topCpu);

            // Add disk info
            if (drives.Any())
            {
                var diskInfo = drives.Select(d => $"{d.Name}:{d.UsedPercent}%").ToList();
                data["env.drives"] = string.Join(", ", diskInfo);
            }

            if (failCpu || failMem || failDisk)
            {
                var reasons = new List<string>();
                if (failCpu) reasons.Add($"CPU>{_options.CpuThresholdPercent}%");
                if (failMem) reasons.Add($"Memory>{_options.MemoryThresholdPercent}%");
                if (failDisk) reasons.Add($"Disk>{_options.DiskThresholdPercent}%");

                return HealthCheckResult.Unhealthy(
                    $"Environment thresholds exceeded: {string.Join(", ", reasons)}",
                    data: data);
            }

            return HealthCheckResult.Healthy(
                $"CPU: {Math.Round(cpuInfo.TotalCpuPercent, 2)}%, Memory: {Math.Round(memPercent, 2)}%",
                data);
        }
        catch (OperationCanceledException)
        {
            return HealthCheckResult.Unhealthy("Environment check cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in environment health check");
            return HealthCheckResult.Unhealthy($"Error: {ex.Message}");
        }
    }

    private static IEnumerable<Process> SafeGetProcesses()
    {
        try { return Process.GetProcesses(); }
        catch { return Enumerable.Empty<Process>(); }
    }

    private static long SafeWS(Process p) { try { return p.WorkingSet64; } catch { return 0; } }
    private static double SafeToGB(long bytes) => Math.Round(bytes / 1024.0 / 1024.0 / 1024.0, 2);

    private static async Task<CpuSample> SampleCpuAsync(int sampleMs, CancellationToken ct)
    {
        var cores = Math.Max(1, global::System.Environment.ProcessorCount);
        var first = SafeGetProcesses().ToDictionary(SafePid, p => SafeCPU(p));
        await Task.Delay(sampleMs, ct);
        var second = SafeGetProcesses().ToDictionary(SafePid, p => SafeCPU(p));

        var perProc = new List<PerProcessCpu>();
        double totalDeltaMs = 0;

        foreach (var kv in second)
        {
            if (!first.TryGetValue(kv.Key, out var prev)) continue;
            var delta = kv.Value.TotalProcessorTimeMs - prev.TotalProcessorTimeMs;
            if (delta < 0) delta = 0;

            totalDeltaMs += delta;
            var cpuPercent = (delta / (sampleMs * cores)) * 100.0;
            perProc.Add(new PerProcessCpu { Pid = kv.Key, Name = kv.Value.Name, CpuPercent = cpuPercent, WorkingSetBytes = kv.Value.WorkingSetBytes });
        }

        var totalCpu = (totalDeltaMs / (sampleMs * cores)) * 100.0;
        return new CpuSample { TotalCpuPercent = totalCpu, PerProcess = perProc };
    }

    private static int SafePid(Process p) { try { return p.Id; } catch { return -1; } }
    private static ProcCpu SafeCPU(Process p)
    {
        try
        {
            return new ProcCpu
            {
                Name = SafeName(p),
                TotalProcessorTimeMs = p.TotalProcessorTime.TotalMilliseconds,
                WorkingSetBytes = SafeWS(p)
            };
        }
        catch { return new ProcCpu { Name = "unknown" }; }
    }

    private static string SafeName(Process p) { try { return p.ProcessName; } catch { return $"pid:{p.Id}"; } }

    private sealed class ProcCpu { public string Name { get; set; } = ""; public double TotalProcessorTimeMs { get; set; } public long WorkingSetBytes { get; set; } }
    private sealed class PerProcessCpu { public int Pid { get; set; } public string Name { get; set; } public double CpuPercent { get; set; } public long WorkingSetBytes { get; set; } }
    private sealed class CpuSample { public double TotalCpuPercent { get; set; } public List<PerProcessCpu> PerProcess { get; set; } = new(); }
}
