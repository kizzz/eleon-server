using Volo.Abp.Domain.Repositories;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.Repositories;
public interface IActionRepository : IBasicRepository<ActionEntity, Guid>
{
  Task<KeyValuePair<int, List<ActionEntity>>> GetListAsync(
          Guid? taskId = null,
          string nameFilter = null,
          string sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = int.MinValue);
}
