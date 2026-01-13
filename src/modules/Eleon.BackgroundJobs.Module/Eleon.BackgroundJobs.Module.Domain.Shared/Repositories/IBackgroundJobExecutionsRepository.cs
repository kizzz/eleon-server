using System;
using Volo.Abp.Domain.Repositories;
using VPortal.BackgroundJobs.Module.Entities;

namespace VPortal.BackgroundJobs.Module.Repositories
{
  public interface IBackgroundJobExecutionsRepository : IBasicRepository<BackgroundJobExecutionEntity, Guid>
  {
  }
}
