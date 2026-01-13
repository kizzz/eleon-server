using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using VPortal.Collaboration.Feature.Module.EntityFrameworkCore;

namespace VPortal.Collaboration.Feature.Module.Module.EntityFrameworkCore;

public class CollaborationDbContextFactory : DefaultDbContextFactoryBase<CollaborationDbContext>
{
  protected override CollaborationDbContext CreateDbContext(
      DbContextOptions<CollaborationDbContext> dbContextOptions)
  {
    return new CollaborationDbContext(dbContextOptions);
  }
}
