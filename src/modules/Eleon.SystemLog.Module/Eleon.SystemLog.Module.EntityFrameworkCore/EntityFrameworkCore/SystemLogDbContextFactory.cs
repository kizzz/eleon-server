using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.DocMessageLog.Module.EntityFrameworkCore
{
  public class SystemLogDbContextFactory : DefaultDbContextFactoryBase<SystemLogDbContext>
  {
    protected override SystemLogDbContext CreateDbContext(
        DbContextOptions<SystemLogDbContext> dbContextOptions)
    {
      return new SystemLogDbContext(dbContextOptions);
    }
  }
}
