using Microsoft.EntityFrameworkCore;
using System.Linq;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounts.Module.Extensions
{
  public static class AccountExtensions
  {
    public static IQueryable<AccountEntity> IncludeDetails(
        this IQueryable<AccountEntity> queryable,
        bool include = true)
    {
      if (!include) return queryable;
      return queryable
          .Include(x => x.Invoices)
          .ThenInclude(x => x.InvoiceRows)
          .Include(x => x.Invoices)
          .ThenInclude(x => x.Receipt)
          .Include(x => x.AccountPackages)
          .Include(x => x.BillingInformation)
          .Include(x => x.Members);
    }
  }
}
