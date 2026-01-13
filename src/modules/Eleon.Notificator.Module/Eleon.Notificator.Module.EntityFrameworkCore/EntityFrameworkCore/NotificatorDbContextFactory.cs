using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.Notificator.Module.EntityFrameworkCore
{
  public class NotificatorDbContextFactory : DefaultDbContextFactoryBase<NotificatorDbContext>
  {
    protected override NotificatorDbContext CreateDbContext(
        DbContextOptions<NotificatorDbContext> dbContextOptions)
    {
      return new NotificatorDbContext(dbContextOptions);
    }
  }
}
