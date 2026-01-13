using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCore
{
  public class EleonAbpModuleDbContextFactory : DefaultDbContextFactoryBase<EleonAbpDbContext>
  {
    protected override EleonAbpDbContext CreateDbContext(
        DbContextOptions<EleonAbpDbContext> dbContextOptions)
    {
      return new EleonAbpDbContext(dbContextOptions);
    }
  }
}
