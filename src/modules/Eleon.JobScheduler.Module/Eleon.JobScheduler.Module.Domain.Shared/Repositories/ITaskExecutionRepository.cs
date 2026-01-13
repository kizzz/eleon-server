using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.Repositories
{
  public interface ITaskExecutionRepository : IBasicRepository<TaskExecutionEntity, Guid>
  {
    Task<KeyValuePair<long, List<TaskExecutionEntity>>> GetListAsync(Guid taskId, int skipCount, int maxCount, string sorting);
  }
}
