using System;
using Volo.Abp.Domain.Repositories;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.Repositories
{
  public interface IActionExecutionRepository : IBasicRepository<ActionExecutionEntity, Guid>
  {

    Task<List<ActionExecutionEntity>> GetListByTaskExecutionIdAsync(Guid taskExecutionId);
  }
}
