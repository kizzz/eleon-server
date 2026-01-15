namespace EventManagementModule.Module.Domain.Shared.Queues;

public interface IQueueEngine
{
  Task EnqueueManyAsync(QueueKey queue, IReadOnlyList<QueueMessageToEnqueue> messages, CancellationToken ct);
  Task<IReadOnlyList<ClaimedQueueMessage>> ClaimManyAsync(QueueKey queue, ClaimOptions options, CancellationToken ct);
  Task<long> GetPendingCountAsync(QueueKey queue, CancellationToken ct);
  Task<bool> ExistsMessageKeyAsync(QueueKey queue, string messageKey, CancellationToken ct);
  Task AckAsync(Guid lockId, IReadOnlyList<Guid> messageIds, CancellationToken ct);
  Task NackAsync(Guid lockId, Guid messageId, NackOptions options, CancellationToken ct);
}
