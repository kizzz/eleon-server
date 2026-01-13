using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using VPortal.ApplicationConfiguration.Module.EntityFrameworkCore;

namespace VPortal.ApplicationConfiguration.Module.Module.EntityFrameworkCore;

public class ApplicationConfigurationDbContextFactory : DefaultDbContextFactoryBase<ApplicationConfigurationDbContext>
{
  protected override ApplicationConfigurationDbContext CreateDbContext(
      DbContextOptions<ApplicationConfigurationDbContext> dbContextOptions)
  {
    return new ApplicationConfigurationDbContext(dbContextOptions);
  }
}
