using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.EntityFrameworkCore;

public class VPortalTenantDbContextFactory : DefaultDbContextFactoryBase<VPortalTenantDbContext>
{
  protected override VPortalTenantDbContext CreateDbContext(
      DbContextOptions<VPortalTenantDbContext> dbContextOptions)
  {
    return new VPortalTenantDbContext(dbContextOptions);
  }
}
