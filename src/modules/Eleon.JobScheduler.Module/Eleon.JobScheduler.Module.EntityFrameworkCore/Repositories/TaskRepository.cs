using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.EntityFrameworkCore;

namespace VPortal.JobScheduler.Module.Repositories
{
  public class TaskRepository :
      EfCoreRepository<JobSchedulerDbContext, TaskEntity, Guid>,
      ITaskRepository
  {
    private readonly IVportalLogger<TaskRepository> logger;

    public TaskRepository(
        IVportalLogger<TaskRepository> logger,
        IDbContextProvider<JobSchedulerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public override async Task<IQueryable<TaskEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync())
          .Include(x => x.Triggers)
          .Include(x => x.Executions)
          .Include(x => x.Actions)
          .ThenInclude(x => x.ParentActions);
    }

    public async Task<List<Guid>> old__GetDueTasksAsync(DateTime dueDate)
    {
      var result = new List<Guid>();
      try
      {
        var dbContext = await GetDbContextAsync();
        var filtered = dbContext.Tasks
            .Where(x => x.NextRunTimeUtc != null && x.NextRunTimeUtc <= dueDate)
            .Select(x => x.Id);
        result = await filtered.ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<KeyValuePair<long, List<TaskEntity>>> GetList(int skipCount, int maxCount, string sorting, string nameFilter)
    {
      var result = default(KeyValuePair<long, List<TaskEntity>>);
      try
      {
        var dbContext = await GetDbContextAsync();
        string namePattern = nameFilter == null ? null : $"%{nameFilter}%";
        var filtered = dbContext.Tasks
            .WhereIf(namePattern != null, x => EF.Functions.Like(x.Name, namePattern) || EF.Functions.Like(x.Id.ToString(), namePattern));
        var paginated = filtered;
        if (!string.IsNullOrWhiteSpace(sorting))
        {
          paginated = paginated.OrderBy(sorting).ThenByDescending(x => x.CreationTime);
        }
        else
        {
          paginated = paginated.OrderByDescending(x => x.CreationTime);
        }
        paginated = paginated
           .Skip(skipCount)
           .Take(maxCount);
        result = new(await filtered.CountAsync(), await paginated.ToListAsync());
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<List<TaskEntity>> GetTasksToStartAsync(DateTime asOfTime)
    {

      try
      {
        var result = await (await GetDbContextAsync())
            .Tasks
            .Include(x => x.Triggers)
            .Include(x => x.Executions)
            .Where(x => x.Status == Common.Module.Constants.JobSchedulerTaskStatus.Ready || (x.AllowForceStop && x.Status == Common.Module.Constants.JobSchedulerTaskStatus.Running))
            .Where(x => x.Triggers.Where(t => t.IsEnabled && (t.ExpireUtc == null || t.ExpireUtc > asOfTime) && t.StartUtc < asOfTime).Any() || x.NextRunTimeUtc < asOfTime)
            .ToListAsync();
        ;
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

    public async Task<TaskEntity> GetWithTriggerAsync(Guid id)
    {

      try
      {
        var result = await (await GetDbContextAsync())
            .Tasks
            .Where(x => x.Id == id)
            .Include(x => x.Triggers)
            .FirstAsync();

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
  }
}
