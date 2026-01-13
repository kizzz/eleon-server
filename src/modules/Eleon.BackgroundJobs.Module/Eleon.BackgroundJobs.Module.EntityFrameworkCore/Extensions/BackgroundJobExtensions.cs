using Microsoft.EntityFrameworkCore;
using System.Linq;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.Extensions
{
  public static class BackgroundJobExtensions
  {
    public static IQueryable<BackgroundJobEntity> IncludeDetails(
        this IQueryable<BackgroundJobEntity> queryable,
        bool include = true)
    {
      if (!include) return queryable;
      return queryable
          .Include(x => x.Executions)
          .ThenInclude(x => x.Messages);
    }

    public static IQueryable<BackgroundJobExecutionEntity> IncludeMessages(
        this IQueryable<BackgroundJobExecutionEntity> queryable,
        bool include = true)
    {
      if (!include) return queryable;
      return queryable
          .Include(x => x.Messages);
    }
  }
}
