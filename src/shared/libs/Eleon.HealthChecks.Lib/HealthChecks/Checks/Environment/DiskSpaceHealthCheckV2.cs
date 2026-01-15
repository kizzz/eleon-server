using EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Environment;

/// <summary>
/// Disk space health check implementing IHealthCheck.
/// Checks file/folder sizes against configured limits.
/// </summary>
public class DiskSpaceHealthCheckV2 : IHealthCheck
{
    private readonly DiskSpaceHealthCheckOptions _options;
    private readonly ILogger<DiskSpaceHealthCheckV2> _logger;

    public DiskSpaceHealthCheckV2(
        IOptions<DiskSpaceHealthCheckOptions> options,
        ILogger<DiskSpaceHealthCheckV2> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_options.Items == null || _options.Items.Count == 0)
        {
            return HealthCheckResult.Healthy("No disk space checks configured", new Dictionary<string, object>
            {
                ["disk.checks_count"] = 0
            });
        }

        var results = new List<PathCheckResult>();
        var sw = Stopwatch.StartNew();

        foreach (var rule in _options.Items)
        {
            cancellationToken.ThrowIfCancellationRequested();

            long maxSizeBytes = rule.MaxSizeMb * 1024 * 1024;
            var result = new PathCheckResult 
            { 
                Path = rule.Path, 
                MaxSizeMb = rule.MaxSizeMb
            };
            var pathSw = Stopwatch.StartNew();

            try
            {
                var absPath = ResolvePath(rule.Path);

                if (File.Exists(absPath))
                {
                    var fi = new FileInfo(absPath);
                    result.SizeBytes = fi.Length;
                    result.Kind = "File";
                    result.Success = result.SizeBytes <= maxSizeBytes;
                }
                else if (Directory.Exists(absPath))
                {
                    var (totalBytes, fileCount) = await ScanDirectoryAsync(absPath, cancellationToken);
                    result.SizeBytes = totalBytes;
                    result.FileCount = fileCount;
                    result.Kind = "Directory";
                    result.Success = totalBytes <= maxSizeBytes;
                }
                else
                {
                    result.Success = false;
                    result.Error = "Path not found";
                }
            }
            catch (OperationCanceledException)
            {
                result.Success = false;
                result.Error = "Operation cancelled";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = $"{ex.GetType().Name}: {ex.Message}";
                _logger.LogWarning(ex, "Disk space check failed for path {Path}", rule.Path);
            }
            finally
            {
                pathSw.Stop();
                result.LatencyMs = pathSw.ElapsedMilliseconds;
                results.Add(result);
            }
        }

        sw.Stop();

        var total = results.Count;
        var ok = results.Count(r => r.Success);
        var failed = total - ok;
        var exceeded = results.Count(r => !r.Success && r.Error == null);

        var data = new Dictionary<string, object>
        {
            ["disk.latency_ms"] = sw.ElapsedMilliseconds,
            ["disk.checks_total"] = total,
            ["disk.checks_ok"] = ok,
            ["disk.checks_failed"] = failed,
            ["disk.checks_exceeded"] = exceeded
        };

        // Add per-path data
        foreach (var r in results)
        {
            var key = $"disk.path_{SanitizeKey(r.Path)}";
            data[$"{key}.success"] = r.Success;
            data[$"{key}.kind"] = r.Kind ?? "Unknown";
            data[$"{key}.size_bytes"] = r.SizeBytes;
            data[$"{key}.max_mb"] = r.MaxSizeMb;
            data[$"{key}.size_mb"] = r.SizeBytes / (1024.0 * 1024.0);
            data[$"{key}.latency_ms"] = r.LatencyMs;

            if (r.FileCount.HasValue)
                data[$"{key}.file_count"] = r.FileCount.Value;

            if (!r.Success && !string.IsNullOrEmpty(r.Error))
                data[$"{key}.error"] = r.Error;
        }

        if (failed == 0)
        {
            return HealthCheckResult.Healthy(
                $"All {total} disk space check(s) passed",
                data);
        }

        if (ok > 0)
        {
            return HealthCheckResult.Degraded(
                $"{ok} of {total} disk space check(s) passed, {failed} failed",
                data: data);
        }

        return HealthCheckResult.Unhealthy(
            $"All {total} disk space check(s) failed",
            data: data);
    }

    private static string ResolvePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return path ?? string.Empty;
        return Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
    }

    private static string SanitizeKey(string path)
    {
        var key = path.Replace(':', '_').Replace('\\', '/');
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(key))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static async Task<(long TotalBytes, long FileCount)> ScanDirectoryAsync(
        string root,
        CancellationToken cancellationToken,
        int maxDepth = 10,
        int currentDepth = 0)
    {
        if (currentDepth >= maxDepth)
            return (0, 0);

        long total = 0;
        long count = 0;

        try
        {
            foreach (var file in Directory.EnumerateFiles(root))
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var fi = new FileInfo(file);
                    total += fi.Length;
                    count++;
                }
                catch { /* ignore */ }
            }

            foreach (var dir in Directory.EnumerateDirectories(root))
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var (subTotal, subCount) = await ScanDirectoryAsync(dir, cancellationToken, maxDepth, currentDepth + 1);
                    total += subTotal;
                    count += subCount;
                }
                catch { /* ignore */ }
            }
        }
        catch { /* ignore */ }

        return (total, count);
    }

    private sealed class PathCheckResult
    {
        public string Path { get; set; } = string.Empty;
        public bool Success { get; set; }
        public long SizeBytes { get; set; }
        public long MaxSizeMb { get; set; }
        public long LatencyMs { get; set; }
        public string? Error { get; set; }
        public string? Kind { get; set; }
        public long? FileCount { get; set; }
    }
}
