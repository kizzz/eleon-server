using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.EntityFrameworkCore;

namespace VPortal.JobScheduler.Module.Repositories;
public class ActionRepository : EfCoreRepository<JobSchedulerDbContext, ActionEntity, Guid>, IActionRepository
{
  public ActionRepository(IDbContextProvider<JobSchedulerDbContext> dbContextProvider) : base(dbContextProvider)
  {
  }

  public async Task<KeyValuePair<int, List<ActionEntity>>> GetListAsync(
          Guid? taskId = null,
          string nameFilter = null,
          string sorting = null,
          int maxResultCount = int.MaxValue,
          int skipCount = int.MinValue)
  {
    var dbSet = await GetQueryableAsync();

    var query = dbSet
        .Include(a => a.Task)
        .Include(a => a.ParentActions)
        .WhereIf(taskId.HasValue, a => a.Task.Id == taskId.Value)
        .WhereIf(!string.IsNullOrWhiteSpace(nameFilter), a => a.DisplayName.Contains(nameFilter) || a.EventName.Contains(nameFilter));

    if (string.IsNullOrEmpty(sorting))
    {
      sorting = nameof(ActionEntity.EventName) + " asc";
    }

    var totalCount = await query.CountAsync();

    query = query
        .OrderBy(sorting)
        .Skip(skipCount)
        .Take(maxResultCount);

    return new KeyValuePair<int, List<ActionEntity>>(
        totalCount,
        await query.ToListAsync()
    );
  }

  public override async Task<IQueryable<ActionEntity>> WithDetailsAsync()
  {
    return (await base.WithDetailsAsync())
        .Include(a => a.ParentActions)
        .Include(a => a.Task)
        ;
  }
}
