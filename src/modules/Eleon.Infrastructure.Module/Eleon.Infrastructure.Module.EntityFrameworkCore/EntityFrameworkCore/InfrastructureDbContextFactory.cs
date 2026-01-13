using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.Infrastructure.Module.EntityFrameworkCore;

public class InfrastructureDbContextFactory : DefaultDbContextFactoryBase<InfrastructureDbContext>
{
  protected override InfrastructureDbContext CreateDbContext(
      DbContextOptions<InfrastructureDbContext> dbContextOptions)
  {
    return new InfrastructureDbContext(dbContextOptions);
  }
}
