using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Application.Contracts.HealthCheck;
using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.HealthCheck;
using HealthCheckModule.Module.Domain.Shared.Constants;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Eleon.Common.Lib.modules.HealthCheck.Module.General;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.CheckDatabase;

public class DatabaseTablesSizeHealthCheck : DefaultHealthCheck
{
  private readonly IConfiguration _configuration;
  private const int TABLES_PER_ENTRY = 500;

  public DatabaseTablesSizeHealthCheck(IConfiguration configuration, IServiceProvider serviceProvider) : base(serviceProvider)
  {
    _configuration = configuration;
  }

  public override string Name => "DatabaseSize";
  public override bool IsPublic => false;

  public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
  {
    var csSection = _configuration.GetSection("ConnectionStrings");
    var pairs = csSection.GetChildren()
        .Select(s => new { Name = s.Key, Value = s.Value })
        .Where(p => !string.IsNullOrWhiteSpace(p.Value))
        .ToList();

    var results = new List<ConnResult>();

    foreach (var p in pairs)
    {
      var r = new ConnResult { Name = p.Name };
      var sw = Stopwatch.StartNew();

      try
      {
        await using var conn = new SqlConnection(p.Value);
        await conn.OpenAsync();
        r.Success = true;

        // Minimal DB info: name + sizes
        r.DbInfo = await ReadSingleAsync(conn, @"
SELECT
  DB_NAME() AS DatabaseName,
  SUM(CASE WHEN type_desc = 'ROWS' THEN CAST(size AS bigint) ELSE 0 END) * 8 AS DataSizeKB,
  SUM(CASE WHEN type_desc = 'LOG'  THEN CAST(size AS bigint) ELSE 0 END) * 8 AS LogSizeKB,
  SUM(CAST(size AS bigint)) * 8 AS TotalSizeKB
FROM sys.database_files;");

        // Tables by size: Schema.Table + TotalSpaceKB (no indexes, no rowcount)
        r.Tables = await ReadManyAsync(conn, @"
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
    AND i.index_id IN (0,1) -- heap or clustered only
  GROUP BY s.name, t.name
)
SELECT
  SchemaName,
  TableName,
  (SchemaName + '.' + TableName) AS FullName,
  TotalSpaceKB
FROM TableSizes
ORDER BY TotalSpaceKB DESC, SchemaName, TableName;");
      }
      catch (Exception exOpen)
      {
        r.Success = false;
        r.OpenError = $"{exOpen.GetType().Name}: {exOpen.Message}";
      }
      finally
      {
        sw.Stop();
        r.ElapsedMs = sw.ElapsedMilliseconds;
        results.Add(r);
      }
    }

    var total = results.Count;
    var ok = results.Count(x => x.Success);
    var failed = total - ok;

    report.Status = failed > 0 ? HealthCheckStatus.Failed : HealthCheckStatus.OK;
    report.Message = total == 0
        ? "No connection strings found under Configuration:ConnectionStrings."
        : $"Database size scan across {total} connection string(s): {ok} reachable, {failed} unreachable.";

    // Emit extras – minimal and separate
    foreach (var r in results)
    {
      AddJson(report, $"Connection_{r.Name}_Status", new
      {
        Name = r.Name,
        Status = r.Success ? "OK" : "FAILED",
        ElapsedMs = r.ElapsedMs,
        Error = r.Success ? null : r.OpenError
      }, r.Success ? ReportInformationSeverity.Info : ReportInformationSeverity.Error);

      if (!r.Success) continue;

      // Minimal DB info
      if (r.DbInfo is not null)
      {
        AddJson(report, $"Db_{r.Name}_Info", new
        {
          DatabaseName = r.DbInfo.TryGet("DatabaseName"),
          TotalSizeKB = r.DbInfo.TryGetLong("TotalSizeKB"),
          DataSizeKB = r.DbInfo.TryGetLong("DataSizeKB"),
          LogSizeKB = r.DbInfo.TryGetLong("LogSizeKB")
        });

        // (Optional) simple numbers as separate entries
        AddSimple(report, $"Db_{r.Name}_TotalSizeKB", r.DbInfo.TryGetLong("TotalSizeKB")?.ToString() ?? "0");
      }

      // Tables by size (chunked)
      if (r.Tables is { Count: > 0 })
      {
        int chunk = 0;
        for (int i = 0; i < r.Tables.Count; i += TABLES_PER_ENTRY)
        {
          var part = r.Tables
              .Skip(i).Take(TABLES_PER_ENTRY)
              .Select(row => new
              {
                Name = row.TryGet("FullName"),
                TotalSpaceKB = row.TryGetLong("TotalSpaceKB")
              })
              .ToList();

          AddJson(report, $"Db_{r.Name}_Tables_{++chunk:000}", part);
        }
        AddSimple(report, $"Db_{r.Name}_Tables_Total", r.Tables.Count.ToString());
      }
    }

    AddSimple(report, "TotalConnections", total.ToString());
    AddSimple(report, "ReachableConnections", ok.ToString());
    AddSimple(report, "UnreachableConnections", failed.ToString());

  }

  // ------------ helpers ------------

  private static async Task<Dictionary<string, object>> ReadSingleAsync(SqlConnection conn, string sql)
  {
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    cmd.CommandTimeout = 60;
    await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
    if (!await reader.ReadAsync()) return new();
    return ReadRow(reader);
  }

  private static async Task<List<Dictionary<string, object>>> ReadManyAsync(SqlConnection conn, string sql)
  {
    var list = new List<Dictionary<string, object>>();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    cmd.CommandTimeout = 120;
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync()) list.Add(ReadRow(reader));
    return list;
  }

  private static Dictionary<string, object> ReadRow(SqlDataReader reader)
  {
    var d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    for (int i = 0; i < reader.FieldCount; i++)
      d[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
    return d;
  }

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

  private sealed class ConnResult
  {
    public string Name { get; set; }
    public bool Success { get; set; }
    public string OpenError { get; set; }
    public long ElapsedMs { get; set; }

    public Dictionary<string, object> DbInfo { get; set; }
    public List<Dictionary<string, object>> Tables { get; set; }
  }
}

internal static class RowExt
{
  public static string TryGet(this Dictionary<string, object> row, string key) =>
      row != null && row.TryGetValue(key, out var v) && v != null ? v.ToString() : null;

  public static long? TryGetLong(this Dictionary<string, object> row, string key)
  {
    if (row == null || !row.TryGetValue(key, out var v) || v == null) return null;
    if (v is long l) return l;
    if (v is int i) return i;
    return long.TryParse(v.ToString(), out var n) ? n : null;
  }
}
