using Microsoft.EntityFrameworkCore;
using System.Linq;
using VPortal.Lifecycle.Feature.Module.Entities;

namespace VPortal.Lifecycle.Feature.Module.Extensions
{
  public static class LifecycleExtensions
  {
    public static IQueryable<StatesGroupTemplateEntity> IncludeDetails(
        this IQueryable<StatesGroupTemplateEntity> queryable,
        bool include = true)
    {
      if (!include)
      {
        return queryable;
      }

      return queryable
          .Include(x => x.States)
          .ThenInclude(x => x.Actors)
          .ThenInclude(x => x.TaskLists);
    }
    public static IQueryable<StatesGroupAuditEntity> IncludeDetails(
        this IQueryable<StatesGroupAuditEntity> queryable,
        bool include = true)
    {
      if (!include)
      {
        return queryable;
      }

      return queryable
          .Include(x => x.States)
          .ThenInclude(x => x.Actors);
    }
  }
}
