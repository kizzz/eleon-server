using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.Auditor.Module.EntityFrameworkCore;

public class AuditorDbContextFactory : DefaultDbContextFactoryBase<AuditorDbContext>
{
  protected override AuditorDbContext CreateDbContext(
      DbContextOptions<AuditorDbContext> dbContextOptions)
  {
    return new AuditorDbContext(dbContextOptions);
  }
}
