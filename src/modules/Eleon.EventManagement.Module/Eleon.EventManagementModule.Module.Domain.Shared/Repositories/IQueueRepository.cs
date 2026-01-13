using EventManagementModule.Module.Domain.Shared.Entities;
using Volo.Abp.Domain.Repositories;

namespace EventManagementModule.Module.Domain.Shared.Repositories;
public interface IQueueRepository : IBasicRepository<QueueEntity, Guid>
{
  Task SetMessagesLimitAsync(
      Guid queueId,
      int limit);

  Task ClearAsync(
      Guid queueId);

  Task EnqueueAsync(
      Guid queueId,
      EventEntity message);

  Task EnqueueManyAsync(
      Guid queueId,
      IList<EventEntity> message);

  Task<EventEntity> DequeueAsync(
      Guid queueId);

  Task<List<EventEntity>> DequeueManyAsync(
      Guid queueId,
      int count);

  Task<QueueEntity> FindByNameAsync(
      string name,
      bool includeDetails = true);

  Task<List<EventEntity>> GetMessagesListAsync(
      Guid queueId,
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      bool includeDetails = true);
}
