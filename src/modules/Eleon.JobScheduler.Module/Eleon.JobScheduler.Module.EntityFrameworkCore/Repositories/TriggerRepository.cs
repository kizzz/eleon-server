using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.EntityFrameworkCore;

namespace VPortal.JobScheduler.Module.Repositories
{
  public class TriggerRepository :
      EfCoreRepository<JobSchedulerDbContext, TriggerEntity, Guid>,
      ITriggerRepository
  {
    private readonly IVportalLogger<TriggerRepository> logger;

    public TriggerRepository(
        IVportalLogger<TriggerRepository> logger,
        IDbContextProvider<JobSchedulerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }
    public override async Task<IQueryable<TriggerEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync());
    }

    public async Task<List<TriggerEntity>> GetListAsync(Guid? taskId, bool? isEnabledFilter = null)
    {
      var result = new List<TriggerEntity>();
      try
      {
        var dbContext = await GetDbContextAsync();
        var filtered = dbContext.Triggers
            .WhereIf(taskId.HasValue, x => x.TaskId == taskId.Value)
            .WhereIf(isEnabledFilter != null, x => x.IsEnabled == isEnabledFilter);
        var paginated = filtered
           .OrderByDescending(x => x.CreationTime);
        result = await paginated.ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<TriggerEntity?> GetNextRunTriggerAsync(Guid taskId)
    {
      var dbSet = await GetDbSetAsync();

      var trigger = await dbSet
          .Where(x => x.TaskId == taskId && x.IsEnabled && x.NextRunUtc != null)
          .OrderBy(x => x.NextRunUtc)
          .FirstOrDefaultAsync();

      return trigger;
    }
  }
}
