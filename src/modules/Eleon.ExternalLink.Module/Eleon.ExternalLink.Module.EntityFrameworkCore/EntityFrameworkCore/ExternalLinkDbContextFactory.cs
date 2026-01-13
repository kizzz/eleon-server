using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.ExternalLink.Module.EntityFrameworkCore
{
  public class ExternalLinkDbContextFactory : DefaultDbContextFactoryBase<ExternalLinkDbContext>
  {
    protected override ExternalLinkDbContext CreateDbContext(
        DbContextOptions<ExternalLinkDbContext> dbContextOptions)
    {
      return new ExternalLinkDbContext(dbContextOptions);
    }
  }
}
