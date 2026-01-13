using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using VPortal.Google.Module.EntityFrameworkCore;

namespace VPortal.Google.Module.Module.EntityFrameworkCore;

public class GoogleDbContextFactory : DefaultDbContextFactoryBase<GoogleDbContext>
{
  protected override GoogleDbContext CreateDbContext(
      DbContextOptions<GoogleDbContext> dbContextOptions)
  {
    return new GoogleDbContext(dbContextOptions);
  }
}
