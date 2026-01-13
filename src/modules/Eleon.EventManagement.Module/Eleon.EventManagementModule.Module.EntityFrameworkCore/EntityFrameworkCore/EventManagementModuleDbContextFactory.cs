using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCore
{
  public class EventManagementModuleDbContextFactory : DefaultDbContextFactoryBase<EventManagementModuleDbContext>
  {
    protected override EventManagementModuleDbContext CreateDbContext(
        DbContextOptions<EventManagementModuleDbContext> dbContextOptions)
    {
      return new EventManagementModuleDbContext(dbContextOptions);
    }
  }
}
