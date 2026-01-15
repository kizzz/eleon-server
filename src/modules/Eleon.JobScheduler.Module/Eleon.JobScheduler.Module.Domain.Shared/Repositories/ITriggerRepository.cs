using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.Repositories
{
  public interface ITriggerRepository : IBasicRepository<TriggerEntity, Guid>
  {
    Task<List<TriggerEntity>> GetListAsync(Guid? taskId, bool? isEnabledFilter = null);
    Task<TriggerEntity?> GetNextRunTriggerAsync(Guid taskId);
  }
}
