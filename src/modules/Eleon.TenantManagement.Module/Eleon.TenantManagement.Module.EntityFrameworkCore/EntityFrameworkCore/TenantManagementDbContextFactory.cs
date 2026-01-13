using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.TenantManagement.Module.EntityFrameworkCore;

public class TenantManagementDbContextFactory : DefaultDbContextFactoryBase<TenantManagementDbContext>
{
  protected override TenantManagementDbContext CreateDbContext(
      DbContextOptions<TenantManagementDbContext> dbContextOptions)
  {
    return new TenantManagementDbContext(dbContextOptions);
  }
}
