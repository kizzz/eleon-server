using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.BackgroundJobs.Module.Entities;

namespace VPortal.BackgroundJobs.Module.Repositories
{
  public interface IBackgroundJobsRepository : IBasicRepository<BackgroundJobEntity, Guid>
  {
    Task<List<BackgroundJobEntity>> GetByType(string type);
    Task<List<BackgroundJobEntity>> GetByDateTime(DateTime now);
    Task<KeyValuePair<long, List<BackgroundJobEntity>>> GetListAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null,
        DateTime? creationDateFilterStart = null,
        DateTime? creationDateFilterEnd = null,
        DateTime? lastExecutionDateFilterStart = null,
        DateTime? lastExecutionDateFilterEnd = null,
        IList<string> typeFilter = null,
        IList<BackgroundJobStatus> statusFilter = null);

    Task<List<TaskIdWithTimeout>> GetLongTimeExecutingJobIdsAsync();

    Task<List<BackgroundJobEntity>> GetRetryJobsAsync(DateTime now);
  }

  public record TaskIdWithTimeout(Guid Id, int TimeoutInMinutes);
}
