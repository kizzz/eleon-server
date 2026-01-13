using Microsoft.EntityFrameworkCore;
using System.Linq;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounts.Module.Extensions
{
  public static class InvoiceExtensions
  {
    public static IQueryable<InvoiceEntity> IncludeDetails(
        this IQueryable<InvoiceEntity> queryable,
        bool include = true)
    {
      if (!include) return queryable;
      return queryable
          .Include(x => x.InvoiceRows);
    }
  }
}
