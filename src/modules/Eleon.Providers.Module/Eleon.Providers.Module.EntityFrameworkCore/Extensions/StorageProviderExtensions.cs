using Microsoft.EntityFrameworkCore;
using System.Linq;
using VPortal.Storage.Module.Entities;

namespace VPortal.Storage.Module.Extensions
{
  public static class StorageProviderExtensions
  {
    public static IQueryable<StorageProviderEntity> IncludeDetails(
        this IQueryable<StorageProviderEntity> queryable,
        bool include = true)
    {
      if (!include) return queryable;
      return queryable
          .Include(x => x.Settings);
    }
  }
}
