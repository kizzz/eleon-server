using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.BackgroundJobs.Module.Entities;
using SharedCollector.modules.Migration.Module.Extensions;

namespace VPortal.BackgroundJobs.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public class BackgroundJobsDbContext : AbpDbContext<BackgroundJobsDbContext>, IBackgroundJobsDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * public DbSet<Question> Questions { get; set; }
   */
  public DbSet<BackgroundJobEntity> EleoncoreBackgroundJobs { get; set; }

  public BackgroundJobsDbContext(DbContextOptions<BackgroundJobsDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureModule();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
