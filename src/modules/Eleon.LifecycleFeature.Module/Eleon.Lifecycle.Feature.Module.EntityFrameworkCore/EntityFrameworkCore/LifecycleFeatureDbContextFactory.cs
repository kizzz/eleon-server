using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.Lifecycle.Feature.Module.EntityFrameworkCore
{
  public class LifecycleFeatureDbContextFactory : DefaultDbContextFactoryBase<LifecycleFeatureDbContext>
  {
    protected override LifecycleFeatureDbContext CreateDbContext(
        DbContextOptions<LifecycleFeatureDbContext> dbContextOptions)
    {
      return new LifecycleFeatureDbContext(dbContextOptions);
    }
  }
}
