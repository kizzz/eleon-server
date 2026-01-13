using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.EntityFrameworkCore;
using VPortal.JobScheduler.Module.Repositories;

namespace JobScheduler.Module.Repositories
{
  public class ActionExecutionRepository :
      EfCoreRepository<JobSchedulerDbContext, ActionExecutionEntity, Guid>,
      IActionExecutionRepository
  {
    private readonly IVportalLogger<ActionExecutionRepository> logger;

    public ActionExecutionRepository(
        IVportalLogger<ActionExecutionRepository> logger,
        IDbContextProvider<JobSchedulerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<List<ActionExecutionEntity>> GetListByTaskExecutionIdAsync(Guid taskExecutionId)
    {
      var result = new List<ActionExecutionEntity>();
      try
      {
        var dbSet = await GetDbSetAsync();

        var query = dbSet.Where(x => x.TaskExecutionId == taskExecutionId);

        result = await query.ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public override async Task<IQueryable<ActionExecutionEntity>> WithDetailsAsync()
    {
      return (await base.WithDetailsAsync()).Include(x => x.Action);
    }
  }
}
