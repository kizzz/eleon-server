using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.SystemServicesModule.Module.EntityFrameworkCore
{
  public class SystemServicesModuleDbContextFactory : DefaultDbContextFactoryBase<SystemServicesModuleDbContext>
  {
    protected override SystemServicesModuleDbContext CreateDbContext(
        DbContextOptions<SystemServicesModuleDbContext> dbContextOptions)
    {
      return new SystemServicesModuleDbContext(dbContextOptions);
    }
  }
}

