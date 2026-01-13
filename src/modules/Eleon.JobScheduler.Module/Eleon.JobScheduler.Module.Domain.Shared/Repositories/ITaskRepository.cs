using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.Repositories
{
  public interface ITaskRepository : IBasicRepository<TaskEntity, Guid>
  {
    Task<KeyValuePair<long, List<TaskEntity>>> GetList(int skipCount, int maxCount, string sorting, string nameFilter);

    [Obsolete("Use get tasks to execute method instead.")]
    Task<List<Guid>> old__GetDueTasksAsync(DateTime dueDate);

    Task<List<TaskEntity>> GetTasksToStartAsync(DateTime asOfTime);

    Task<TaskEntity> GetWithTriggerAsync(Guid id);
  }
}
