using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.SitesManagement.Module.EntityFrameworkCore;

public class SitesManagementDbContextFactory : DefaultDbContextFactoryBase<SitesManagementDbContext>
{
  protected override SitesManagementDbContext CreateDbContext(
      DbContextOptions<SitesManagementDbContext> dbContextOptions)
  {
    return new SitesManagementDbContext(dbContextOptions);
  }
}

