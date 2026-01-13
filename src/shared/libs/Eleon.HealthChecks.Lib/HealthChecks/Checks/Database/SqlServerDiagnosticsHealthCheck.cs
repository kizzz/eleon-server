using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;

/// <summary>
/// SQL Server diagnostics health check for expensive operations.
/// Requires diag tag and feature flag. Uses TTL caching.
/// </summary>
public class SqlServerDiagnosticsHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly SqlServerHealthCheckOptions _options;
    private readonly ILogger<SqlServerDiagnosticsHealthCheck> _logger;

    // Cache for expensive diagnostics
    private static readonly ConcurrentDictionary<string, (DateTime ExpiresAt, Dictionary<string, object> Data)> _cache = new();

    public SqlServerDiagnosticsHealthCheck(
        IConfiguration configuration,
        IOptions<SqlServerHealthCheckOptions> options,
        ILogger<SqlServerDiagnosticsHealthCheck> logger)
    {
        _configuration = configuration;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (!_options.EnableDiagnostics)
        {
            return HealthCheckResult.Healthy("Diagnostics disabled", new Dictionary<string, object>
            {
                ["sql.diagnostics.enabled"] = false
            });
        }

        var csSection = _configuration.GetSection("ConnectionStrings");
        var connections = csSection.GetChildren()
            .Select(s => new { Name = s.Key, Value = s.Value })
            .Where(p => !string.IsNullOrWhiteSpace(p.Value))
            .ToList();

        if (connections.Count == 0)
        {
            return HealthCheckResult.Healthy("No connection strings configured", new Dictionary<string, object>
            {
                ["sql.diagnostics.connections_count"] = 0
            });
        }

        var results = new List<DiagnosticsResult>();
        var sw = Stopwatch.StartNew();

        foreach (var conn in connections)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new DiagnosticsResult { Name = conn.Name };
            var connSw = Stopwatch.StartNew();

            try
            {
                // Check cache
                var cacheKey = $"sql-diag-{conn.Name}";
                if (_cache.TryGetValue(cacheKey, out var cached) && cached.ExpiresAt > DateTime.UtcNow)
                {
                    result.Success = true;
                    result.FromCache = true;
                    result.Data = cached.Data;
                    _logger.LogDebug("Using cached diagnostics for {ConnectionName}", conn.Name);
                }
                else
                {
                    // Run diagnostics
                    await using var sqlConn = new SqlConnection(conn.Value);
                    await sqlConn.OpenAsync(cancellationToken);

                    // Get database size info
                    var dbInfo = await GetDatabaseInfoAsync(sqlConn, cancellationToken);
                    
                    // Get top tables by size (limited)
                    var topTables = await GetTopTablesAsync(sqlConn, _options.MaxTablesInDiagnostics, cancellationToken);

                    result.Data = new Dictionary<string, object>
                    {
                        ["database_name"] = dbInfo.GetValueOrDefault("DatabaseName")?.ToString() ?? "unknown",
                        ["total_size_kb"] = dbInfo.GetValueOrDefault("TotalSizeKB") ?? 0L,
                        ["data_size_kb"] = dbInfo.GetValueOrDefault("DataSizeKB") ?? 0L,
                        ["log_size_kb"] = dbInfo.GetValueOrDefault("LogSizeKB") ?? 0L,
                        ["tables_count"] = topTables.Count
                    };

                    // Add top tables (limited)
                    if (topTables.Any())
                    {
                        var tablesList = topTables.Take(10).Select(t => 
                            $"{t.GetValueOrDefault("SchemaName")}.{t.GetValueOrDefault("TableName")}:{t.GetValueOrDefault("TotalSpaceKB")}KB").ToList();
                        result.Data["top_tables"] = string.Join(", ", tablesList);
                    }

                    // Cache result
                    var expiresAt = DateTime.UtcNow.AddMinutes(_options.DiagnosticsCacheMinutes);
                    _cache.AddOrUpdate(cacheKey, (expiresAt, result.Data), (k, v) => (expiresAt, result.Data));

                    result.Success = true;
                    result.FromCache = false;
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
                _logger.LogWarning(ex, "SQL diagnostics check failed for connection {ConnectionName}", conn.Name);
            }
            finally
            {
                connSw.Stop();
                result.LatencyMs = connSw.ElapsedMilliseconds;
                results.Add(result);
            }
        }

        sw.Stop();

        var total = results.Count;
        var ok = results.Count(r => r.Success);
        var failed = total - ok;
        var fromCache = results.Count(r => r.FromCache);

        var data = new Dictionary<string, object>
        {
            ["sql.diagnostics.latency_ms"] = sw.ElapsedMilliseconds,
            ["sql.diagnostics.connections_total"] = total,
            ["sql.diagnostics.connections_ok"] = ok,
            ["sql.diagnostics.connections_failed"] = failed,
            ["sql.diagnostics.from_cache"] = fromCache
        };

        // Add per-connection data
        foreach (var r in results)
        {
            var prefix = $"sql.diagnostics.connection_{r.Name}";
            data[$"{prefix}.success"] = r.Success;
            data[$"{prefix}.latency_ms"] = r.LatencyMs;
            data[$"{prefix}.from_cache"] = r.FromCache;

            if (r.Success && r.Data != null)
            {
                foreach (var kvp in r.Data)
                {
                    data[$"{prefix}.{kvp.Key}"] = kvp.Value;
                }
            }
            else if (!r.Success && !string.IsNullOrEmpty(r.Error))
            {
                data[$"{prefix}.error"] = r.Error;
            }
        }

        if (failed == 0)
        {
            return HealthCheckResult.Healthy(
                $"All {total} SQL Server diagnostics check(s) completed",
                data);
        }

        if (ok > 0)
        {
            return HealthCheckResult.Degraded(
                $"{ok} of {total} SQL Server diagnostics check(s) completed, {failed} failed",
                data: data);
        }

        return HealthCheckResult.Unhealthy(
            $"All {total} SQL Server diagnostics check(s) failed",
            data: data);
    }

    private static async Task<Dictionary<string, object>> GetDatabaseInfoAsync(SqlConnection conn, CancellationToken ct)
    {
        const string sql = @"
SELECT
  DB_NAME() AS DatabaseName,
  SUM(CASE WHEN type_desc = 'ROWS' THEN CAST(size AS bigint) ELSE 0 END) * 8 AS DataSizeKB,
  SUM(CASE WHEN type_desc = 'LOG'  THEN CAST(size AS bigint) ELSE 0 END) * 8 AS LogSizeKB,
  SUM(CAST(size AS bigint)) * 8 AS TotalSizeKB
FROM sys.database_files;";

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandTimeout = 30;

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                dict[name] = value ?? 0;
            }
            return dict;
        }

        return new Dictionary<string, object>();
    }

    private static async Task<List<Dictionary<string, object>>> GetTopTablesAsync(
        SqlConnection conn,
        int maxTables,
        CancellationToken ct)
    {
        const string sql = @"
WITH TableSizes AS (
  SELECT
    s.name  AS SchemaName,
    t.name  AS TableName,
    SUM(CAST(a.total_pages AS bigint)) * 8 AS TotalSpaceKB
  FROM sys.tables t
  JOIN sys.schemas s       ON s.schema_id = t.schema_id
  JOIN sys.indexes i       ON i.object_id = t.object_id
  JOIN sys.partitions p    ON p.object_id = t.object_id AND p.index_id = i.index_id
  JOIN sys.allocation_units a ON a.container_id = p.partition_id
  WHERE t.is_ms_shipped = 0
    AND i.index_id IN (0,1)
  GROUP BY s.name, t.name
)
SELECT TOP (@MaxTables)
  SchemaName,
  TableName,
  TotalSpaceKB
FROM TableSizes
ORDER BY TotalSpaceKB DESC;";

        var list = new List<Dictionary<string, object>>();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandTimeout = 60;
        cmd.Parameters.Add(new SqlParameter("@MaxTables", maxTables));

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                dict[name] = value ?? 0;
            }
            list.Add(dict);
        }

        return list;
    }

    private sealed class DiagnosticsResult
    {
        public string Name { get; set; } = string.Empty;
        public bool Success { get; set; }
        public long LatencyMs { get; set; }
        public bool FromCache { get; set; }
        public string? Error { get; set; }
        public Dictionary<string, object>? Data { get; set; }
    }
}
