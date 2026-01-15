namespace EventManagementModule.Module.Domain.Shared.Queues;

public sealed record QueueKey(Guid QueueId, Guid? TenantId, byte Lane = 0);

public sealed record QueueMessageToEnqueue(
    string Name,
    ReadOnlyMemory<byte> Payload,
    string ContentType,
    string? Encoding,
    string? MessageKey,
    string? TraceId);

public sealed record ClaimedQueueMessage(
    Guid Id,
    Guid QueueId,
    byte Lane,
    long EnqueueSeq,
    string Name,
    int Attempts,
    DateTime CreatedUtc,
    string? MessageKey,
    string? TraceId,
    ReadOnlyMemory<byte> Payload,
    string ContentType,
    string? Encoding);

public sealed record ClaimOptions(
    int Count,
    Guid LockId,
    TimeSpan LockFor);

public sealed record NackOptions(
    int MaxAttempts,
    TimeSpan Delay,
    string Error);
