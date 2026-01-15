using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.Repositories
{
  public interface ITaskExecutionRepository : IBasicRepository<TaskExecutionEntity, Guid>
  {
    Task<KeyValuePair<long, List<TaskExecutionEntity>>> GetListAsync(Guid taskId, int skipCount, int maxCount, string sorting);
    Task<TaskExecutionEntity?> GetNewestByStartedAtAsync(Guid taskId);
    Task<TaskExecutionEntity?> GetNewestByFinishedAtAsync(Guid taskId);
  }
}
