using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public class JobSchedulerDbContext : AbpDbContext<JobSchedulerDbContext>, IJobSchedulerDbContext
{
  public DbSet<TaskEntity> Tasks { get; set; }
  public DbSet<TaskExecutionEntity> TaskExecutions { get; set; }
  public DbSet<TriggerEntity> Triggers { get; set; }
  public DbSet<ActionEntity> Actions { get; set; }

  public JobSchedulerDbContext(DbContextOptions<JobSchedulerDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureJobScheduler();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
