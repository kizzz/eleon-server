using EleonsoftSdk.modules.HealthCheck.Module.Checks.SystemCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            var absPath = ResolvePath(rule.Path);
            var result = new PathCheckResult 
            { 
                RelativePath = rule.Path,
                AbsolutePath = absPath,
                MaxSizeMb = rule.MaxSizeMb
            };
            var pathSw = Stopwatch.StartNew();

            try
            {
                if (File.Exists(absPath))
                {
                    var fi = new FileInfo(absPath);
                    result.SizeBytes = fi.Length;
                    result.Success = result.SizeBytes <= maxSizeBytes;
                }
                else if (Directory.Exists(absPath))
                {
                    var (totalBytes, fileCount) = await ScanDirectoryAsync(absPath, cancellationToken);
                    result.SizeBytes = totalBytes;
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
                results.Add(result);
            }
        }

        sw.Stop();

        var total = results.Count;
        var ok = results.Count(r => r.Success);
        var failed = total - ok;

        var data = new Dictionary<string, object>();

        // Add per-path data - only full path and size
        int i = 1;
        foreach (var r in results)
        {
            data[$"[{i}].path"] = r.AbsolutePath;
            data[$"[{i}].size_mb"] = (long)(r.SizeBytes / (1024.0 * 1024.0));
            i++;
        }

        // Build message with relative path and size current/max
        var pathMessages = results.Select(r =>
        {
            var sizeMb = r.SizeBytes / (1024.0 * 1024.0);
            return $"{r.RelativePath}: {sizeMb:F2}MB/{r.MaxSizeMb}MB";
        });

        var message = string.Join("; ", pathMessages);

        if (failed == 0)
        {
            return HealthCheckResult.Healthy(message, data);
        }

        if (ok > 0)
        {
            return HealthCheckResult.Degraded(message, data: data);
        }

        return HealthCheckResult.Unhealthy(message, data: data);
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
        public string RelativePath { get; set; } = string.Empty;
        public string AbsolutePath { get; set; } = string.Empty;
        public bool Success { get; set; }
        public long SizeBytes { get; set; }
        public long MaxSizeMb { get; set; }
        public string? Error { get; set; }
    }
}
