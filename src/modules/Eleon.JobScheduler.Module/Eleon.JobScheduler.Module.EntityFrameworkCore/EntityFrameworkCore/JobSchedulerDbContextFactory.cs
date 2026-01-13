using EleonsoftSdk.modules.Migration.Module;
using Microsoft.EntityFrameworkCore;

namespace VPortal.JobScheduler.Module.EntityFrameworkCore;

public class JobSchedulerDbContextFactory : DefaultDbContextFactoryBase<JobSchedulerDbContext>
{
  protected override JobSchedulerDbContext CreateDbContext(
      DbContextOptions<JobSchedulerDbContext> dbContextOptions)
  {
    return new JobSchedulerDbContext(dbContextOptions);
  }
}
