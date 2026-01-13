using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;
using VPortal.BackgroundJobs.Module.EntityFrameworkCore;

namespace BackgroundJobs.Module.EntityFrameworkCore
{
  public class BackgroundJobsDbContextFactory : DefaultDbContextFactoryBase<BackgroundJobsDbContext>
  {
    protected override BackgroundJobsDbContext CreateDbContext(
        DbContextOptions<BackgroundJobsDbContext> dbContextOptions)
    {
      return new BackgroundJobsDbContext(dbContextOptions);
    }
  }
}
