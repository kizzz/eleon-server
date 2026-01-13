using EventManagementModule.Module.Domain.Shared.Entities;
using Volo.Abp.Domain.Repositories;

namespace EventManagementModule.Module.Domain.Shared.Repositories;
public interface IQueueDefinitionRepository : IBasicRepository<QueueDefinitionEntity, Guid>
{
  Task<List<QueueDefinitionEntity>> GetCustomListAsync(
      Guid? tenantId,
      string sorting = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      bool includeDetails = true,
      CancellationToken cancellationToken = default);
}
