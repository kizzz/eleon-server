using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Database;

/// <summary>
/// Safe SQL Server readiness check that:
/// - Uses hardcoded safe query (no custom SQL)
/// - Only reads metadata (no DB creation, no writes, no business data)
/// - Honors CancellationToken
/// - Returns structured observations
/// </summary>
public class SqlServerReadinessHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SqlServerReadinessHealthCheck> _logger;

    // Hardcoded safe query - no custom SQL allowed
    private const string SafeReadinessQuery = @"
SELECT
  1 AS ok,
  DB_NAME() AS db,
  @@SERVERNAME AS server_name,
  CAST(SERVERPROPERTY('ProductVersion') AS nvarchar(50)) AS version;";

    public SqlServerReadinessHealthCheck(
        IConfiguration configuration,
        ILogger<SqlServerReadinessHealthCheck> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var csSection = _configuration.GetSection("ConnectionStrings");
        var connections = csSection.GetChildren()
            .Select(s => new { Name = s.Key, Value = s.Value })
            .Where(p => !string.IsNullOrWhiteSpace(p.Value))
            .ToList();

        if (connections.Count == 0)
        {
            return HealthCheckResult.Healthy("No connection strings configured", new Dictionary<string, object>
            {
                ["sql.connections_count"] = 0
            });
        }

        var results = new List<ConnectionResult>();
        var sw = Stopwatch.StartNew();

        foreach (var conn in connections)
        {
            var result = new ConnectionResult { Name = conn.Name };
            var connSw = Stopwatch.StartNew();

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using var sqlConn = new SqlConnection(conn.Value);
                await sqlConn.OpenAsync(cancellationToken);

                await using var cmd = sqlConn.CreateCommand();
                cmd.CommandText = SafeReadinessQuery;
                cmd.CommandTimeout = 5; // Short timeout for readiness

                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    result.Success = true;
                    result.DatabaseName = reader.IsDBNull(1) ? null : reader.GetString(1);
                    result.ServerName = reader.IsDBNull(2) ? null : reader.GetString(2);
                    result.Version = reader.IsDBNull(3) ? null : reader.GetString(3);
                }
            }
            catch (OperationCanceledException)
            {
                result.Success = false;
                result.Error = "Operation cancelled";
                _logger.LogWarning("SQL readiness check cancelled for connection {ConnectionName}", conn.Name);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = $"{ex.GetType().Name}: {ex.Message}";
                _logger.LogWarning(ex, "SQL readiness check failed for connection {ConnectionName}", conn.Name);
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

        var data = new Dictionary<string, object>
        {
            ["sql.latency_ms"] = sw.ElapsedMilliseconds,
            ["sql.connections_total"] = total,
            ["sql.connections_ok"] = ok,
            ["sql.connections_failed"] = failed
        };

        // Add per-connection data
        foreach (var r in results)
        {
            var prefix = $"sql.connection_{r.Name}";
            data[$"{prefix}.success"] = r.Success;
            data[$"{prefix}.latency_ms"] = r.LatencyMs;

            if (r.Success)
            {
                if (!string.IsNullOrEmpty(r.DatabaseName))
                    data[$"{prefix}.db"] = r.DatabaseName;
                if (!string.IsNullOrEmpty(r.ServerName))
                    data[$"{prefix}.server"] = r.ServerName;
                if (!string.IsNullOrEmpty(r.Version))
                    data[$"{prefix}.version"] = r.Version;
            }
            else
            {
                data[$"{prefix}.error"] = r.Error ?? "Unknown error";
            }
        }

        if (failed == 0)
        {
            return HealthCheckResult.Healthy(
                $"All {total} SQL Server connection(s) are healthy",
                data);
        }

        if (ok > 0)
        {
            return HealthCheckResult.Degraded(
                $"{ok} of {total} SQL Server connection(s) are healthy, {failed} failed",
                data: data);
        }

        return HealthCheckResult.Unhealthy(
            $"All {total} SQL Server connection(s) failed",
            data: data);
    }

    private sealed class ConnectionResult
    {
        public string Name { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Error { get; set; }
        public long LatencyMs { get; set; }
        public string? DatabaseName { get; set; }
        public string? ServerName { get; set; }
        public string? Version { get; set; }
    }
}
