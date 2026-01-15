using System.Data;
using System.Data.Common;
using System.IO.Compression;
using System.Linq;
using EventManagementModule.Module.Domain.Shared.Queues;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VPortal.EventManagementModule.Module.EntityFrameworkCore;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCore.Queues;

public class SqlClaimQueueEngine : IQueueEngine
{
  private const byte PendingStatus = 0;
  private const byte ProcessingStatus = 1;
  private const byte DeadStatus = 3;

  private readonly EventManagementModuleDbContext _dbContext;
  private readonly QueueEngineOptions _options;
  private readonly IVportalLogger<SqlClaimQueueEngine> _logger;

  public SqlClaimQueueEngine(
      EventManagementModuleDbContext dbContext,
      IOptions<QueueEngineOptions> options,
      IVportalLogger<SqlClaimQueueEngine> logger)
  {
    _dbContext = dbContext;
    _options = options.Value;
    _logger = logger;
  }

  public async Task EnqueueManyAsync(QueueKey queue, IReadOnlyList<QueueMessageToEnqueue> messages, CancellationToken ct)
  {
    if (messages == null || messages.Count == 0)
    {
      return;
    }

    await _dbContext.Database.OpenConnectionAsync(ct);
    await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
    try
    {
      var connection = _dbContext.Database.GetDbConnection();
      var dbTransaction = transaction.GetDbTransaction();

      var ids = messages.Select(_ => Guid.NewGuid()).ToList();
      await InsertMessagesAsync(connection, dbTransaction, queue, messages, ids, ct);
      await InsertBodiesAsync(connection, dbTransaction, messages, ids, ct);

      await transaction.CommitAsync(ct);
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync(ct);
      _logger.CaptureAndSuppress(ex);
      throw;
    }
  }

  public async Task<IReadOnlyList<ClaimedQueueMessage>> ClaimManyAsync(QueueKey queue, ClaimOptions options, CancellationToken ct)
  {
    var claimed = new List<ClaimedQueueMessage>();
    await _dbContext.Database.OpenConnectionAsync(ct);
    var connection = _dbContext.Database.GetDbConnection();

    var (metadata, reclaimedCount) = await ClaimMetadataAsync(connection, queue, options, ct);
    if (metadata.Count == 0)
    {
      return claimed;
    }

    var bodies = await LoadBodiesAsync(connection, metadata.Select(m => m.Id).ToList(), ct);
    foreach (var item in metadata)
    {
      if (!bodies.TryGetValue(item.Id, out var body))
      {
        continue;
      }

      claimed.Add(item with
      {
        Payload = body.Payload,
        ContentType = body.ContentType,
        Encoding = body.Encoding
      });
    }

    _logger.Log.LogInformation(
        "Claimed {Count} messages (reclaimed {Reclaimed}) for queue {QueueId} lane {Lane} lock {LockId}",
        claimed.Count,
        reclaimedCount,
        queue.QueueId,
        queue.Lane,
        options.LockId);

    return claimed;
  }

  public async Task<long> GetPendingCountAsync(QueueKey queue, CancellationToken ct)
  {
    await _dbContext.Database.OpenConnectionAsync(ct);
    var connection = _dbContext.Database.GetDbConnection();

    using var command = connection.CreateCommand();
    command.CommandText = @"
SELECT COUNT_BIG(*)
FROM EcEventQueueMessages
WHERE QueueId = @QueueId
  AND Lane = @Lane
  AND Status = @PendingStatus
  AND (VisibleAfterUtc IS NULL OR VisibleAfterUtc <= SYSUTCDATETIME());";
    AddParameter(command, "@QueueId", queue.QueueId);
    AddParameter(command, "@Lane", queue.Lane);
    AddParameter(command, "@PendingStatus", PendingStatus);

    var result = await command.ExecuteScalarAsync(ct);
    return result == null ? 0 : Convert.ToInt64(result);
  }

  public async Task<bool> ExistsMessageKeyAsync(QueueKey queue, string messageKey, CancellationToken ct)
  {
    await _dbContext.Database.OpenConnectionAsync(ct);
    var connection = _dbContext.Database.GetDbConnection();

    using var command = connection.CreateCommand();
    command.CommandText = @"
SELECT TOP (1) 1
FROM EcEventQueueMessages
WHERE QueueId = @QueueId
  AND Lane = @Lane
  AND MessageKey = @MessageKey;";
    AddParameter(command, "@QueueId", queue.QueueId);
    AddParameter(command, "@Lane", queue.Lane);
    AddParameter(command, "@MessageKey", messageKey);

    var result = await command.ExecuteScalarAsync(ct);
    return result != null;
  }

  public async Task AckAsync(Guid lockId, IReadOnlyList<Guid> messageIds, CancellationToken ct)
  {
    if (messageIds == null || messageIds.Count == 0)
    {
      return;
    }

    await _dbContext.Database.OpenConnectionAsync(ct);
    var connection = _dbContext.Database.GetDbConnection();

    using var command = connection.CreateCommand();
    command.CommandText = BuildDeleteByLockAndIdsSql(messageIds.Count);
    AddParameter(command, "@LockId", lockId);
    for (int i = 0; i < messageIds.Count; i++)
    {
      AddParameter(command, $"@Id{i}", messageIds[i]);
    }

    await command.ExecuteNonQueryAsync(ct);
    _logger.Log.LogInformation("Acked {Count} messages for lock {LockId}", messageIds.Count, lockId);
  }

  public async Task NackAsync(Guid lockId, Guid messageId, NackOptions options, CancellationToken ct)
  {
    await _dbContext.Database.OpenConnectionAsync(ct);
    var connection = _dbContext.Database.GetDbConnection();

    using var command = connection.CreateCommand();
    command.CommandText = @"
UPDATE EcEventQueueMessages
SET
  Status = CASE WHEN Attempts >= @MaxAttempts THEN @DeadStatus ELSE @PendingStatus END,
  LockId = NULL,
  LockedUntilUtc = NULL,
  VisibleAfterUtc = CASE WHEN Attempts >= @MaxAttempts THEN NULL ELSE DATEADD(SECOND, @DelaySeconds, SYSUTCDATETIME()) END,
  LastError = @Error
WHERE Id = @Id AND LockId = @LockId;";

    AddParameter(command, "@MaxAttempts", options.MaxAttempts);
    AddParameter(command, "@DelaySeconds", (int)options.Delay.TotalSeconds);
    AddParameter(command, "@DeadStatus", DeadStatus);
    AddParameter(command, "@PendingStatus", PendingStatus);
    AddParameter(command, "@Error", options.Error);
    AddParameter(command, "@Id", messageId);
    AddParameter(command, "@LockId", lockId);

    await command.ExecuteNonQueryAsync(ct);
    _logger.Log.LogWarning(
        "Nack message {MessageId} for lock {LockId} (delay {DelaySeconds}s maxAttempts {MaxAttempts})",
        messageId,
        lockId,
        (int)options.Delay.TotalSeconds,
        options.MaxAttempts);
  }

  private async Task InsertMessagesAsync(
      DbConnection connection,
      DbTransaction transaction,
      QueueKey queue,
      IReadOnlyList<QueueMessageToEnqueue> messages,
      IReadOnlyList<Guid> ids,
      CancellationToken ct)
  {
    using var command = connection.CreateCommand();
    command.Transaction = transaction;

    command.CommandText = BuildInsertMessagesSql(messages.Count);
    AddParameter(command, "@QueueId", queue.QueueId);
    AddParameter(command, "@TenantId", queue.TenantId);
    AddParameter(command, "@Lane", queue.Lane);
    AddParameter(command, "@Status", PendingStatus);

    for (int i = 0; i < messages.Count; i++)
    {
      var message = messages[i];
      AddParameter(command, $"@Id{i}", ids[i]);
      AddParameter(command, $"@Name{i}", message.Name);
      AddParameter(command, $"@MessageKey{i}", message.MessageKey);
      AddParameter(command, $"@TraceId{i}", message.TraceId);
    }

    await command.ExecuteNonQueryAsync(ct);
  }

  private async Task InsertBodiesAsync(
      DbConnection connection,
      DbTransaction transaction,
      IReadOnlyList<QueueMessageToEnqueue> messages,
      IReadOnlyList<Guid> ids,
      CancellationToken ct)
  {
    using var command = connection.CreateCommand();
    command.Transaction = transaction;

    command.CommandText = BuildInsertBodiesSql(messages.Count);
    for (int i = 0; i < messages.Count; i++)
    {
      var message = messages[i];
      var payloadBytes = _options.EnableCompression
          ? Compress(message.Payload.ToArray())
          : message.Payload.ToArray();

      AddParameter(command, $"@Id{i}", ids[i]);
      AddParameter(command, $"@Payload{i}", payloadBytes);
      AddParameter(command, $"@ContentType{i}", message.ContentType);
      AddParameter(command, $"@Encoding{i}", message.Encoding);
    }

    await command.ExecuteNonQueryAsync(ct);
  }

  private async Task<(List<ClaimedQueueMessage> Messages, int ReclaimedCount)> ClaimMetadataAsync(
      DbConnection connection,
      QueueKey queue,
      ClaimOptions options,
      CancellationToken ct)
  {
    using var command = connection.CreateCommand();
    command.CommandText = @"
DECLARE @now datetime2(3) = SYSUTCDATETIME();

;WITH candidates AS (
    SELECT TOP (@Count) *
    FROM EcEventQueueMessages WITH (READPAST, UPDLOCK, ROWLOCK)
    WHERE QueueId = @QueueId
      AND Lane = @Lane
      AND (
           (Status = @PendingStatus AND (VisibleAfterUtc IS NULL OR VisibleAfterUtc <= @now))
        OR (Status = @ProcessingStatus AND LockedUntilUtc IS NOT NULL AND LockedUntilUtc < @now)
      )
    ORDER BY
      CASE WHEN Status = @PendingStatus THEN 0 ELSE 1 END,
      EnqueueSeq
)
UPDATE candidates
SET
    Status = @ProcessingStatus,
    LockId = @LockId,
    LockedUntilUtc = DATEADD(SECOND, @LockSeconds, @now),
    Attempts = Attempts + 1
OUTPUT
    inserted.Id,
    inserted.QueueId,
    inserted.Lane,
    inserted.EnqueueSeq,
    inserted.Name,
    inserted.Attempts,
    inserted.CreatedUtc,
    inserted.MessageKey,
    inserted.TraceId,
    deleted.Status;";

    AddParameter(command, "@Count", options.Count);
    AddParameter(command, "@QueueId", queue.QueueId);
    AddParameter(command, "@Lane", queue.Lane);
    AddParameter(command, "@PendingStatus", PendingStatus);
    AddParameter(command, "@ProcessingStatus", ProcessingStatus);
    AddParameter(command, "@LockId", options.LockId);
    AddParameter(command, "@LockSeconds", (int)options.LockFor.TotalSeconds);

    var results = new List<ClaimedQueueMessage>();
    var reclaimedCount = 0;
    using var reader = await command.ExecuteReaderAsync(ct);
    while (await reader.ReadAsync(ct))
    {
      var previousStatus = reader.GetByte(9);
      if (previousStatus == ProcessingStatus)
      {
        reclaimedCount++;
      }

      results.Add(new ClaimedQueueMessage(
          reader.GetGuid(0),
          reader.GetGuid(1),
          reader.GetByte(2),
          reader.GetInt64(3),
          reader.GetString(4),
          reader.GetInt32(5),
          reader.GetDateTime(6),
          reader.IsDBNull(7) ? null : reader.GetString(7),
          reader.IsDBNull(8) ? null : reader.GetString(8),
          ReadOnlyMemory<byte>.Empty,
          "application/json",
          null));
    }

    return (results, reclaimedCount);
  }

  private async Task<Dictionary<Guid, (ReadOnlyMemory<byte> Payload, string ContentType, string? Encoding)>> LoadBodiesAsync(
      DbConnection connection,
      IReadOnlyList<Guid> ids,
      CancellationToken ct)
  {
    if (ids.Count == 0)
    {
      return new Dictionary<Guid, (ReadOnlyMemory<byte>, string, string?)>();
    }

    using var command = connection.CreateCommand();
    command.CommandText = BuildSelectBodiesSql(ids.Count);
    for (int i = 0; i < ids.Count; i++)
    {
      AddParameter(command, $"@Id{i}", ids[i]);
    }

    var result = new Dictionary<Guid, (ReadOnlyMemory<byte>, string, string?)>();
    using var reader = await command.ExecuteReaderAsync(ct);
    while (await reader.ReadAsync(ct))
    {
      var id = reader.GetGuid(0);
      var payload = (byte[])reader.GetValue(1);
      var contentType = reader.GetString(2);
      var encoding = reader.IsDBNull(3) ? null : reader.GetString(3);
      if (_options.EnableCompression)
      {
        payload = Decompress(payload);
      }

      result[id] = (payload, contentType, encoding);
    }

    return result;
  }

  private static string BuildInsertMessagesSql(int count)
  {
    var values = Enumerable.Range(0, count)
        .Select(i => $"(@Id{i}, @TenantId, @QueueId, @Lane, @Name{i}, @Status, @MessageKey{i}, @TraceId{i})");

    return $@"
INSERT INTO EcEventQueueMessages
  (Id, TenantId, QueueId, Lane, Name, Status, MessageKey, TraceId)
VALUES {string.Join(", ", values)};";
  }

  private static string BuildInsertBodiesSql(int count)
  {
    var values = Enumerable.Range(0, count)
        .Select(i => $"(@Id{i}, @Payload{i}, @ContentType{i}, @Encoding{i})");

    return $@"
INSERT INTO EcEventQueueMessageBodies
  (Id, Payload, ContentType, Encoding)
VALUES {string.Join(", ", values)};";
  }

  private static string BuildSelectBodiesSql(int count)
  {
    var ids = Enumerable.Range(0, count).Select(i => $"@Id{i}");
    return $@"
SELECT Id, Payload, ContentType, Encoding
FROM EcEventQueueMessageBodies
WHERE Id IN ({string.Join(", ", ids)});";
  }

  private static string BuildDeleteByLockAndIdsSql(int count)
  {
    var ids = Enumerable.Range(0, count).Select(i => $"@Id{i}");
    return $@"
DELETE m
FROM EcEventQueueMessages m
WHERE m.LockId = @LockId
  AND m.Id IN ({string.Join(", ", ids)});";
  }

  private static void AddParameter(DbCommand command, string name, object? value)
  {
    var parameter = command.CreateParameter();
    parameter.ParameterName = name;
    parameter.Value = value ?? DBNull.Value;
    command.Parameters.Add(parameter);
  }

  private static byte[] Compress(byte[] input)
  {
    using var output = new MemoryStream();
    using (var gzip = new GZipStream(output, CompressionLevel.Fastest, leaveOpen: true))
    {
      gzip.Write(input, 0, input.Length);
    }
    return output.ToArray();
  }

  private static byte[] Decompress(byte[] input)
  {
    using var inputStream = new MemoryStream(input);
    using var gzip = new GZipStream(inputStream, CompressionMode.Decompress);
    using var output = new MemoryStream();
    gzip.CopyTo(output);
    return output.ToArray();
  }
}
