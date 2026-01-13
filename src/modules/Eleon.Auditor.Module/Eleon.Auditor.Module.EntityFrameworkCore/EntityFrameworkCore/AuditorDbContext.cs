using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Auditor.Module.Entities;
using SharedCollector.modules.Migration.Module.Extensions;
using Microsoft.Extensions.Logging;

namespace VPortal.Auditor.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public class AuditorDbContext : AbpDbContext<AuditorDbContext>, IAuditorDbContext
{
  public DbSet<AuditCurrentVersionEntity> AuditCurrentVersions { get; set; }
  public DbSet<AuditHistoryRecordEntity> AuditHistoryRecords { get; set; }
  public DbSet<AuditDataEntity> AuditData { get; set; }

  public AuditorDbContext(DbContextOptions<AuditorDbContext> options)
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
