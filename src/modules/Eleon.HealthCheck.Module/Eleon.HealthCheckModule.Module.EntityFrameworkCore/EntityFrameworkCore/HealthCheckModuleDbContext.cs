using EleonsoftModuleCollector.HealthCheckModule.HealthCheckModule.Module.Domain.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.HealthCheckModule.Module.EntityFrameworkCore;

[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public class HealthCheckModuleDbContext : AbpDbContext<HealthCheckModuleDbContext>, IHealthCheckModuleDbContext
{
  public DbSet<HealthCheck> HealthChecks { get; set; }
  public DbSet<HealthCheckReport> HealthCheckReports { get; set; }

  public HealthCheckModuleDbContext(DbContextOptions<HealthCheckModuleDbContext> options)
      : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<HealthCheckReport>()
        .HasIndex(x => x.HealthCheckId);

    builder.Entity<HealthCheckReport>()
        .HasMany(x => x.ExtraInformation)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);

    builder.Entity<HealthCheckReport>(b =>
    {
      b.Property(x => x.UpTime)
           .HasConversion(
               v => v.Ticks,               // TimeSpan -> long
               v => TimeSpan.FromTicks(v)); // long -> TimeSpan
    });

    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
