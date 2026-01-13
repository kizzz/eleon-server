using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using VPortal.Identity.Module.EntityFrameworkCore;

namespace VPortal.Identity.Module.Module.EntityFrameworkCore;

public class IdentityDbContextFactory : DefaultDbContextFactoryBase<IdentityDbContext>
{
  protected override IdentityDbContext CreateDbContext(
      DbContextOptions<IdentityDbContext> dbContextOptions)
  {
    return new IdentityDbContext(dbContextOptions);
  }
}
