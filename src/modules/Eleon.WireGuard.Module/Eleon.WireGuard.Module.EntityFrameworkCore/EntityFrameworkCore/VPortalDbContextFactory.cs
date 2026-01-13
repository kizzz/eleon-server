using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.EntityFrameworkCore;

public class VPortalDbContextFactory : DefaultDbContextFactoryBase<VPortalDbContext>
{
  protected override VPortalDbContext CreateDbContext(
      DbContextOptions<VPortalDbContext> dbContextOptions)
  {
    return new VPortalDbContext(dbContextOptions);
  }
}
