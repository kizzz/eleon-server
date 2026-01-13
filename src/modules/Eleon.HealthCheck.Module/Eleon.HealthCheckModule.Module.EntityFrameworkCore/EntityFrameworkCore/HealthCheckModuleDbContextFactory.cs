using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.HealthCheckModule.Module.EntityFrameworkCore
{
  public class HealthCheckModuleDbContextFactory : DefaultDbContextFactoryBase<HealthCheckModuleDbContext>
  {
    protected override HealthCheckModuleDbContext CreateDbContext(
        DbContextOptions<HealthCheckModuleDbContext> dbContextOptions)
    {
      return new HealthCheckModuleDbContext(dbContextOptions);
    }
  }
}
