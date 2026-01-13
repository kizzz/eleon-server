using Microsoft.EntityFrameworkCore;
using System.Linq;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounts.Module.Extensions
{
  public static class PackageTemplateExtensions
  {
    public static IQueryable<PackageTemplateEntity> IncludeDetails(
        this IQueryable<PackageTemplateEntity> queryable,
        bool include = true)
    {
      if (!include) return queryable;
      return queryable
          .Include(x => x.PackageTemplateModules);
    }
  }
}
