using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.Accounting.Module.EntityFrameworkCore
{
  public class AccountingDbContextFactory : DefaultDbContextFactoryBase<AccountingDbContext>
  {
    protected override AccountingDbContext CreateDbContext(
        DbContextOptions<AccountingDbContext> dbContextOptions)
    {
      return new AccountingDbContext(dbContextOptions);
    }
  }
}
