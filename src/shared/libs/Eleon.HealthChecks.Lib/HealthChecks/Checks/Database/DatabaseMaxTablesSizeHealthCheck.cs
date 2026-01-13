using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Eleon.Common.Lib.modules.HealthCheck.Module.General;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.CheckDatabase;

// ---------------- Options ----------------

public sealed class DatabaseMaxTablesSizeOptions
{
  /// <summary>Per-connection table rules. Connection name must match a key under Configuration["ConnectionStrings"].</summary>
  public List<ConnectionRule> Connections { get; set; } = new();
}

public sealed class ConnectionRule
{
  /// <summary>ConnectionStrings key.</summary>
  public string Name { get; set; } = default!;

  /// <summary>Tables to check for this connection.</summary>
  public List<TableRule> Tables { get; set; } = new();
}

public sealed class TableRule
{
  /// <summary>Table name in "schema.table" or "table" (defaults to dbo).</summary>
  public string Name { get; set; } = default!;

  /// <summary>Allowed maximum size in KB. If actual > AllowedMaxKB -> FAIL.</summary>
  public long AllowedMaxKB { get; set; }
}

// ---------------- Check ----------------

public class DatabaseMaxTablesSizeHealthCheck : DefaultHealthCheck
{
  private const int CommandTimeoutSeconds = 120;

  private readonly IConfiguration _configuration;
  private readonly DatabaseMaxTablesSizeOptions _opt;

  public DatabaseMaxTablesSizeHealthCheck(
      IConfiguration configuration,
      IServiceProvider serviceProvider,
      IOptions<DatabaseMaxTablesSizeOptions>? options = null) : base(serviceProvider)
  {
    _configuration = configuration;
    _opt = options?.Value ?? new DatabaseMaxTablesSizeOptions();
  }

  public override string Name => "DatabaseMaxTables";
  public override bool IsPublic => true;

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    if (_opt.Connections is not { Count: > 0 })
    {
      AddSimple(report, "Warning_NoRulesProvided",
          "No connection/table rules provided in options.",
          ReportInformationSeverity.Warning);
      report.Status = HealthCheckStatus.OK;
      report.Message = "No rules to check.";
      return;
    }

    var csSection = _configuration.GetSection("ConnectionStrings");
    var allConn = csSection.GetChildren()
        .Where(s => !string.IsNullOrWhiteSpace(s.Value))
        .ToDictionary(s => s.Key, s => s.Value!);

    int totalConn = 0, okConn = 0, missingConn = 0, errorConn = 0;
    var exceededTables = new List<string>();

    foreach (var connRule in _opt.Connections)
    {
      totalConn++;

      if (!allConn.TryGetValue(connRule.Name, out var connStr))
      {
        AddSimple(report, $"Connection_{connRule.Name}_Warning",
            $"Connection string '{connRule.Name}' not found in configuration.",
            ReportInformationSeverity.Warning);
        missingConn++;
        continue;
      }

      var sw = Stopwatch.StartNew();
      try
      {
        // Build target list (schema, table) from rules
        var targets = new List<(string Schema, string Table, TableRule Rule)>();
        foreach (var tr in connRule.Tables)
        {
          var (sch, tbl) = SplitFullName(tr.Name);
          targets.Add((sch, tbl, tr));
        }

        // Query actual sizes for the requested tables (heap/clustered only)
        var actualSizes = await ReadTableSizesAsync(connStr, targets, CommandTimeoutSeconds);

        // Emit per-table results
        foreach (var tgt in targets)
        {
          if (actualSizes.TryGetValue((tgt.Schema, tgt.Table), out var sizeKb))
          {
            AddJson(report, $"Db_{connRule.Name}_{tgt.Schema}.{tgt.Table}",
                new
                {
                  Name = $"{tgt.Schema}.{tgt.Table}",
                  SizeKB = sizeKb,
                  AllowedMaxKB = tgt.Rule.AllowedMaxKB,
                  Status = sizeKb > tgt.Rule.AllowedMaxKB ? "EXCEEDED" : "OK"
                });

            if (sizeKb > tgt.Rule.AllowedMaxKB)
            {
              exceededTables.Add($"{tgt.Schema}.{tgt.Table}");
              AddSimple(report, $"Db_{connRule.Name}_{tgt.Schema}.{tgt.Table}_Exceeded",
                  $"Table '{tgt.Schema}.{tgt.Table}' exceeds limit: SizeKB={sizeKb}, AllowedKB={tgt.Rule.AllowedMaxKB}",
                  ReportInformationSeverity.Error);
            }
          }
          else
          {
            // Not found
            AddSimple(report, $"Db_{connRule.Name}_{tgt.Schema}.{tgt.Table}_Missing",
                $"Table '{tgt.Schema}.{tgt.Table}' not found.",
                ReportInformationSeverity.Warning);
          }
        }

        sw.Stop();
        AddJson(report, $"Connection_{connRule.Name}_Status",
            new { Name = connRule.Name, Status = "OK", ElapsedMs = sw.ElapsedMilliseconds });

        okConn++;
      }
      catch (Exception ex)
      {
        sw.Stop();
        errorConn++;
        AddJson(report, $"Connection_{connRule.Name}_Status",
            new
            {
              Name = connRule.Name,
              Status = "FAILED",
              ElapsedMs = sw.ElapsedMilliseconds,
              Error = $"{ex.GetType().Name}: {ex.Message}"
            },
            ReportInformationSeverity.Error);
      }
    }

    report.Status = exceededTables.Any() ? HealthCheckStatus.Failed : HealthCheckStatus.OK;
    report.Message = $"Per-table limits across {totalConn} connection(s): {okConn} OK, {missingConn} missing, {errorConn} errors."
                     + (exceededTables.Any() ? $" One or more tables exceeded allowed size. ({string.Join(", ", exceededTables)})" : "");

    AddSimple(report, "Connections_Total", totalConn.ToString());
    AddSimple(report, "Connections_OK", okConn.ToString());
    AddSimple(report, "Connections_Missing", missingConn.ToString(),
        missingConn > 0 ? ReportInformationSeverity.Warning : ReportInformationSeverity.Info);
    AddSimple(report, "Connections_Errors", errorConn.ToString(),
        errorConn > 0 ? ReportInformationSeverity.Error : ReportInformationSeverity.Info);

  }

  // ------------- SQL helpers -------------

  private static (string Schema, string Table) SplitFullName(string full)
  {
    if (string.IsNullOrWhiteSpace(full)) return ("dbo", "");
    var idx = full.IndexOf('.');
    if (idx <= 0) return ("dbo", full.Trim());
    return (full[..idx].Trim(), full[(idx + 1)..].Trim());
  }

  private static async Task<Dictionary<(string Schema, string Table), long>> ReadTableSizesAsync(
      string connStr,
      List<(string Schema, string Table, TableRule Rule)> targets,
      int commandTimeoutSeconds)
  {
    var result = new Dictionary<(string, string), long>(StringTupleComparer.OrdinalIgnoreCase);
    if (targets.Count == 0) return result;

    await using var conn = new SqlConnection(connStr);
    await conn.OpenAsync();

    // Build @targets table variable with parameters to avoid injection.
    var valuesSql = new List<string>();
    var parameters = new List<SqlParameter>();
    for (int i = 0; i < targets.Count; i++)
    {
      var ps = new SqlParameter($"@s{i}", SqlDbType.NVarChar, 128) { Value = targets[i].Schema };
      var pt = new SqlParameter($"@t{i}", SqlDbType.NVarChar, 128) { Value = targets[i].Table };
      parameters.Add(ps);
      parameters.Add(pt);
      valuesSql.Add($"(@s{i}, @t{i})");
    }

    var sql = $@"
DECLARE @targets TABLE (SchemaName sysname, TableName sysname);
INSERT INTO @targets (SchemaName, TableName) VALUES {string.Join(", ", valuesSql)};

WITH TableSizes AS (
  SELECT
    s.name  AS SchemaName,
    t.name  AS TableName,
    SUM(CAST(a.total_pages AS bigint)) * 8 AS TotalSpaceKB
  FROM sys.tables t
  JOIN sys.schemas s          ON s.schema_id = t.schema_id
  JOIN sys.indexes i          ON i.object_id = t.object_id
  JOIN sys.partitions p       ON p.object_id = t.object_id AND p.index_id = i.index_id
  JOIN sys.allocation_units a ON a.container_id = p.partition_id
  WHERE t.is_ms_shipped = 0
    AND i.index_id IN (0,1) -- heap or clustered only
  GROUP BY s.name, t.name
)
SELECT ts.SchemaName, ts.TableName, ts.TotalSpaceKB
FROM TableSizes ts
JOIN @targets tt
  ON tt.SchemaName = ts.SchemaName AND tt.TableName = ts.TableName;";

    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    cmd.CommandTimeout = Math.Max(30, commandTimeoutSeconds);
    cmd.Parameters.AddRange(parameters.ToArray());

    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
      var schema = reader.GetString(0);
      var table = reader.GetString(1);
      var kb = reader.IsDBNull(2) ? 0L : Convert.ToInt64(reader.GetValue(2));
      result[(schema, table)] = kb;
    }

    return result;
  }

  // ------------- report helpers -------------

  private static void AddSimple(HealthCheckReportEto report, string key, string value,
      ReportInformationSeverity severity = ReportInformationSeverity.Info)
  {
    report.ExtraInformation.Add(new ReportExtraInformationEto
    {
      Key = key,
      Value = value,
      Severity = severity,
      Type = HealthCheckDefaults.ExtraInfoTypes.Simple
    });
  }

  private static void AddJson(HealthCheckReportEto report, string key, object value,
      ReportInformationSeverity severity = ReportInformationSeverity.Info)
  {
    report.ExtraInformation.Add(new ReportExtraInformationEto
    {
      Key = key,
      Value = JsonSerializer.Serialize(value),
      Severity = severity,
      Type = HealthCheckDefaults.ExtraInfoTypes.Json
    });
  }

  private sealed class StringTupleComparer : IEqualityComparer<(string A, string B)>
  {
    public static readonly StringTupleComparer OrdinalIgnoreCase = new();
    public bool Equals((string A, string B) x, (string A, string B) y) =>
        string.Equals(x.A, y.A, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(x.B, y.B, StringComparison.OrdinalIgnoreCase);
    public int GetHashCode((string A, string B) obj) =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(obj.A) * 397 ^
        StringComparer.OrdinalIgnoreCase.GetHashCode(obj.B);
  }
}
