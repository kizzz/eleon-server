using BackgroundJobs.Module.Extensions;
using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.BackgroundJobs.Module.Entities;
using VPortal.BackgroundJobs.Module.EntityFrameworkCore;

namespace VPortal.BackgroundJobs.Module.Repositories
{

  public class BackgroundJobsRepository : EfCoreRepository<BackgroundJobsDbContext, BackgroundJobEntity, Guid>, IBackgroundJobsRepository
  {
    private readonly IDbContextProvider<BackgroundJobsDbContext> dbContextProvider;
    private readonly IVportalLogger<BackgroundJobsRepository> logger;
    public BackgroundJobsRepository(IDbContextProvider<BackgroundJobsDbContext> dbContextProvider, IVportalLogger<BackgroundJobsRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public async Task<List<BackgroundJobEntity>> GetByDateTime(DateTime now)
    {

      List<BackgroundJobEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await
            dbSet
            .Where(job => job.Status == BackgroundJobStatus.New && job.ScheduleExecutionDateUtc < now)
            .ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<BackgroundJobEntity>> GetRetryJobsAsync(DateTime now)
    {

      try
      {
        var dbSet = await GetDbSetAsync();
        var result = await dbSet
            .Where(job =>
                job.RetryIntervalInMinutes > 0 &&
                job.MaxRetryAttempts > 0 &&
                job.CurrentRetryAttempt < job.MaxRetryAttempts &&
                job.NextRetryTimeUtc != null &&
                job.NextRetryTimeUtc <= now
            )
            .ToListAsync();
        return result;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
    }

    public async Task<List<BackgroundJobEntity>> GetByType(string type)
    {
      List<BackgroundJobEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet
            .Where(backgroundJob => backgroundJob.Type == type)
            .ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<KeyValuePair<long, List<BackgroundJobEntity>>> GetListAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null,
        DateTime? creationDateFilterStart = null,
        DateTime? creationDateFilterEnd = null,
        DateTime? lastExecutionDateFilterStart = null,
        DateTime? lastExecutionDateFilterEnd = null,
        IList<string> typeFilter = null,
        IList<BackgroundJobStatus> statusFilter = null)
    {
      KeyValuePair<long, List<BackgroundJobEntity>> result = new();
      try
      {
        var dbContext = await GetDbContextAsync();
        string pattern = searchQuery == null ? null : $"%{searchQuery}%";
        var filtered = dbContext.EleoncoreBackgroundJobs
            .WhereIf(typeFilter != null, x => typeFilter.Contains(x.Type))
            .WhereIf(statusFilter != null, x => statusFilter.Contains(x.Status))
            .WhereIf(creationDateFilterStart != null, x => x.CreationTime.Date >= creationDateFilterStart.Value.Date)
            .WhereIf(creationDateFilterEnd != null, x => x.CreationTime.Date <= creationDateFilterEnd.Value.Date)
            .WhereIf(lastExecutionDateFilterStart != null, x => x.LastExecutionDateUtc.Date >= lastExecutionDateFilterStart.Value.Date)
            .WhereIf(lastExecutionDateFilterEnd != null, x => x.LastExecutionDateUtc.Date <= lastExecutionDateFilterEnd.Value.Date)
            .WhereIf(searchQuery != null,
                x =>
                EF.Functions.Like(x.Description, pattern) ||
                EF.Functions.Like(x.StartExecutionParams, pattern));
        var paginated = filtered
           .OrderBy(sorting)
           .ThenByDescending(x => x.LastExecutionDateUtc)
           .Skip(skipCount)
           .Take(maxResultCount)
           .IncludeDetails();
        result = new(await filtered.CountAsync(), await paginated.ToListAsync());
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<List<TaskIdWithTimeout>> GetLongTimeExecutingJobIdsAsync()
    {
      var dbSet = await GetDbSetAsync();

      var now = DateTime.UtcNow;

      var query = dbSet
      .Where(x => x.Status == BackgroundJobStatus.Executing || x.Status == BackgroundJobStatus.Retring)
      .Where(x => x.TimeoutInMinutes > 0)
      .Where(x => x.Executions
          .Any(e =>
          (e.Status == BackgroundJobExecutionStatus.Starting || e.Status == BackgroundJobExecutionStatus.Started) &&
          e.ExecutionStartTimeUtc.AddMinutes(x.TimeoutInMinutes) < now))
      .Select(x => new TaskIdWithTimeout(x.Id, x.TimeoutInMinutes));

      return await query.ToListAsync();
    }

    public override async Task<IQueryable<BackgroundJobEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync()).IncludeDetails();
    }
  }
}
