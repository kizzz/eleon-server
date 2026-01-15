using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.EntityFrameworkCore;

namespace VPortal.JobScheduler.Module.Repositories
{
  public class TaskExecutionRepository :
      EfCoreRepository<JobSchedulerDbContext, TaskExecutionEntity, Guid>,
      ITaskExecutionRepository
  {
    private readonly IVportalLogger<TaskExecutionRepository> logger;

    public TaskExecutionRepository(
        IVportalLogger<TaskExecutionRepository> logger,
        IDbContextProvider<JobSchedulerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public override async Task<IQueryable<TaskExecutionEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync())
          .Include(x => x.ActionExecutions)
          .ThenInclude(x => x.ParentActionExecutions);
    }

    public async Task<KeyValuePair<long, List<TaskExecutionEntity>>> GetListAsync(Guid taskId, int skipCount, int maxCount, string sorting)
    {
      var result = default(KeyValuePair<long, List<TaskExecutionEntity>>);
      try
      {
        var dbContext = await GetDbContextAsync();

        var query = (await WithDetailsAsync())
            .Where(x => x.TaskId == taskId);

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(sorting))
        {
          query = query
              .OrderBy(sorting)
              .ThenByDescending(x => x.CreationTime);
        }
        else
        {
          query = query
              .OrderByDescending(x => x.CreationTime);
        }

        query = query
           .Skip(skipCount)
           .Take(maxCount);

        result = new(totalCount, await query.ToListAsync());
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<TaskExecutionEntity> GetNewestByStartedAtAsync(Guid taskId)
    {
      return await (await WithDetailsAsync())
        .Where(x => x.TaskId == taskId)
          .OrderByDescending(x => x.StartedAtUtc ?? DateTime.MinValue)
          .FirstOrDefaultAsync();
    }

    public async Task<TaskExecutionEntity> GetNewestByFinishedAtAsync(Guid taskId)
    {
      return await (await WithDetailsAsync())
        .Where(x => x.TaskId == taskId)
          .OrderByDescending(x => x.FinishedAtUtc ?? DateTime.MinValue)
          .FirstOrDefaultAsync();
    }
  }
}
