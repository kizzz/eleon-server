using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCore
{
  public class EleoncoreTenantManagementModuleDbContextFactory : DefaultDbContextFactoryBase<EleoncoreTenantManagementDbContext>
  {
    protected override EleoncoreTenantManagementDbContext CreateDbContext(
        DbContextOptions<EleoncoreTenantManagementDbContext> dbContextOptions)
    {
      return new EleoncoreTenantManagementDbContext(dbContextOptions);
    }
  }
}
