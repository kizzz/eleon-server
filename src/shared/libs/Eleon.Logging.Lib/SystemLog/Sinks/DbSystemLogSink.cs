using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Logging.Lib.SystemLog.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eleon.Logging.Lib.SystemLog.Sinks;
public sealed class DbSystemLogSink : ISystemLogSink, IDisposable
{
  private readonly string _connectionString;
  private readonly string _tableName = "[dbo].[EcSystemLogs]";
  private readonly object _lock = new();
  private SqlConnection? _conn;
  private bool _disposed;

  // NEW: window in minutes to deduplicate by hash
  private readonly int _groupingTimeMinutes;

  public DbSystemLogSink(
      string connectionString,
      string tableName = "[dbo].[EcSystemLogs]",
      int groupingTimeMinutes = 24 * 60)
  {
    _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    _tableName = tableName;
    _groupingTimeMinutes = groupingTimeMinutes > 0 ? groupingTimeMinutes : 24 * 60;
  }

  public async Task WriteAsync(IReadOnlyList<SystemLogEntry> batch, CancellationToken ct)
  {
    if (_disposed || batch is null || batch.Count == 0) return;

    var conn = await GetOpenConnectionAsync(ct);

    // Optional: a single transaction per batch provides better throughput
    using var tx = conn.BeginTransaction();

    try
    {
      foreach (var e in batch)
      {
        await UpsertAsync(conn, tx, e, ct);
      }

      tx.Commit();
    }
    catch
    {
      try { tx.Rollback(); } catch { /* ignore */ }

      // reconnect once and retry the whole batch (best-effort)
      ResetConnection();
      conn = await GetOpenConnectionAsync(ct);

      using var tx2 = conn.BeginTransaction();
      foreach (var e in batch)
      {
        await UpsertAsync(conn, tx2, e, ct);
      }
      tx2.Commit();
    }
  }

  // ---------- internals ----------
  private async Task UpsertAsync(SqlConnection conn, SqlTransaction tx, SystemLogEntry e, CancellationToken ct)
  {
    var creationTime = e.Time == default ? DateTime.UtcNow : e.Time;

    // Pull tenant from extra props if provided
    var tenantIdStr = e.ExtraProperties.GetValueOrDefault("TenantId");
    Guid? tenantId = Guid.TryParse(tenantIdStr, out var tGuid) ? tGuid : null;

    // Stable hash based on level + message + tenant
    var hash = EleonsoftLogging.GenerateHash(e.LogLevel, e.Message ?? string.Empty, tenantId, e.ApplicationName ?? string.Empty);

    // Increment the latest matching row in the window if any
    const string updateSql = @"
;WITH candidate AS (
    SELECT TOP (1) Id
    FROM {TABLE} WITH (UPDLOCK, ROWLOCK)
    WHERE [Hash] = @hash
      AND [CreationTime] >= @thresholdUtc
      AND [IsArchived] = 0
    ORDER BY [CreationTime] DESC
)
UPDATE {TABLE}
   SET [Count] = [Count] + 1,
       [LastModificationTime] = SYSUTCDATETIME()
WHERE Id IN (SELECT Id FROM candidate);
SELECT @@ROWCOUNT;";

    using (var cmd = new SqlCommand(updateSql.Replace("{TABLE}", _tableName), conn, tx))
    {
      var threshold = creationTime.AddMinutes(-_groupingTimeMinutes);

      cmd.Parameters.Add(new SqlParameter("@hash", SqlDbType.NVarChar, 128) { Value = hash });
      cmd.Parameters.Add(new SqlParameter("@thresholdUtc", SqlDbType.DateTime2) { Value = threshold });

      var updated = (int)(await cmd.ExecuteScalarAsync(ct) ?? 0);

      if (updated > 0)
        return; // deduped successfully
    }

    // No existing row in window. Insert a new one with Count = 1.
    const string insertSql = @"
INSERT INTO {TABLE}
(
    [Id],
    [TenantId],
    [Message],
    [LogLevel],
    [ApplicationName],
    [InitiatorId],
    [InitiatorType],
    [ExtraProperties],
    [ConcurrencyStamp],
    [CreationTime],
    [CreatorId],
    [LastModificationTime],
    [LastModifierId],
    [IsDeleted],
    [DeleterId],
    [DeletionTime],
    [IsArchived],
    [Count],
    [Hash]
)
VALUES
(
    @id,
    @tenantId,
    @message,
    @logLevel,
    @appName,
    @initiatorId,
    @initiatorType,
    @extraJson,
    @concurrency,
    @creationUtc,
    NULL,
    NULL,
    NULL,
    0,
    NULL,
    NULL,
    @isArchived,
    @count,
    @hash
);";

    using (var cmd = new SqlCommand(insertSql.Replace("{TABLE}", _tableName), conn, tx))
    {
      var id = Guid.NewGuid();
      var concurrency = GenerateConcurrencyStamp40();
      var extraJson = SerializeProps(e.ExtraProperties);
      var initiatorId = e.ExtraProperties.GetValueOrDefault("InitiatorId");
      var initiatorType = e.ExtraProperties.GetValueOrDefault("InitiatorType");

      cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.UniqueIdentifier) { Value = id });
      cmd.Parameters.Add(new SqlParameter("@tenantId", SqlDbType.UniqueIdentifier) { Value = (object?)tenantId ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@message", SqlDbType.NVarChar, -1) { Value = (object?)e.Message ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@logLevel", SqlDbType.Int) { Value = (int)e.LogLevel });
      cmd.Parameters.Add(new SqlParameter("@appName", SqlDbType.NVarChar, 256) { Value = (object?)e.ApplicationName ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@initiatorId", SqlDbType.NVarChar, 128) { Value = (object?)initiatorId ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@initiatorType", SqlDbType.NVarChar, 128) { Value = (object?)initiatorType ?? DBNull.Value });
      cmd.Parameters.Add(new SqlParameter("@extraJson", SqlDbType.NVarChar, -1) { Value = extraJson });
      cmd.Parameters.Add(new SqlParameter("@concurrency", SqlDbType.NVarChar, 64) { Value = concurrency });
      cmd.Parameters.Add(new SqlParameter("@creationUtc", SqlDbType.DateTime2) { Value = creationTime });
      cmd.Parameters.Add(new SqlParameter("@isArchived", SqlDbType.Bit) { Value = e.LogLevel == SystemLogLevel.Info });
      cmd.Parameters.Add(new SqlParameter("@count", SqlDbType.Int) { Value = 1 });
      cmd.Parameters.Add(new SqlParameter("@hash", SqlDbType.NVarChar, 128) { Value = hash });

      await cmd.ExecuteNonQueryAsync(ct);
    }
  }

  private async Task<SqlConnection> GetOpenConnectionAsync(CancellationToken ct)
  {
    if (_disposed) throw new ObjectDisposedException(nameof(DbSystemLogSink));

    var c = _conn;
    if (c is { State: ConnectionState.Open }) return c;

    lock (_lock)
    {
      _conn ??= new SqlConnection(_connectionString);
      c = _conn;
    }

    if (c.State != ConnectionState.Open)
      await c.OpenAsync(ct);

    return c;
  }

  private void ResetConnection()
  {
    lock (_lock)
    {
      try { _conn?.Dispose(); } catch { /* ignore */ }
      _conn = null;
    }
  }

  private async Task BulkCopyAsync(SqlConnection conn, DataTable table, CancellationToken ct)
  {
    using var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, null)
    {
      DestinationTableName = _tableName,
      BatchSize = table.Rows.Count,
      BulkCopyTimeout = 60
    };

    // map by name to be safe
    foreach (DataColumn col in table.Columns)
      bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);

    await bulk.WriteToServerAsync(table, ct);
  }

  private static readonly JsonSerializerOptions _jsonOpts = new()
  {
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false
  };

  private static DataTable BuildDataTable(IReadOnlyList<SystemLogEntry> batch)
  {
    var dt = new DataTable();

    dt.Columns.Add("Id", typeof(Guid));
    dt.Columns.Add("TenantId", typeof(Guid));
    dt.Columns.Add("Message", typeof(string));
    dt.Columns.Add("LogLevel", typeof(int));
    dt.Columns.Add("ApplicationName", typeof(string));
    dt.Columns.Add("InitiatorId", typeof(string));
    dt.Columns.Add("InitiatorType", typeof(string));
    dt.Columns.Add("ExtraProperties", typeof(string));
    dt.Columns.Add("ConcurrencyStamp", typeof(string));
    dt.Columns.Add("CreationTime", typeof(DateTime));
    dt.Columns.Add("CreatorId", typeof(Guid));
    dt.Columns.Add("LastModificationTime", typeof(DateTime));
    dt.Columns.Add("LastModifierId", typeof(Guid));
    dt.Columns.Add("IsDeleted", typeof(bool));
    dt.Columns.Add("DeleterId", typeof(Guid));
    dt.Columns.Add("DeletionTime", typeof(DateTime));
    dt.Columns.Add("IsArchived", typeof(bool));

    foreach (var e in batch)
    {
      var id = Guid.NewGuid();
      var concurrency = GenerateConcurrencyStamp40();
      var creationTime = e.Time == default ? DateTime.UtcNow : e.Time;

      var extraJson = SerializeProps(e.ExtraProperties);

      var tenantId = e.ExtraProperties.GetValueOrDefault("TenantId");
      var hasTenantId = Guid.TryParse(tenantId, out var tenantGuid);

      var initiatorId = e.ExtraProperties.GetValueOrDefault("InitiatorId");
      var initiatorType = e.ExtraProperties.GetValueOrDefault("InitiatorType");

      dt.Rows.Add(
          id,
          hasTenantId ? tenantGuid : DBNull.Value,
          e.Message ?? string.Empty,
          (int)e.LogLevel,
          (object?)e.ApplicationName ?? DBNull.Value,
          (object?)initiatorId ?? DBNull.Value,
          (object?)initiatorType ?? DBNull.Value,
          extraJson,
          concurrency,
          creationTime,
          DBNull.Value,   // CreatorId
          DBNull.Value,   // LastModificationTime
          DBNull.Value,   // LastModifierId
          false,          // IsDeleted
          DBNull.Value,   // DeleterId
          DBNull.Value,    // DeletionTime
          e.LogLevel == SystemLogLevel.Info
      );
    }

    return dt;
  }

  private static string SerializeProps(Dictionary<string, string>? props)
      => props is null || props.Count == 0
          ? "{}"
          : JsonSerializer.Serialize(props, _jsonOpts);

  private static string GenerateConcurrencyStamp40()
  {
    Span<byte> bytes = stackalloc byte[20]; // 20 bytes -> 40 hex chars
    RandomNumberGenerator.Fill(bytes);
    var sb = new StringBuilder(40);
    foreach (var b in bytes) sb.Append(b.ToString("x2"));
    return sb.ToString();
  }

  public void Dispose()
  {
    if (_disposed) return;
    _disposed = true;
    ResetConnection();
  }
}
