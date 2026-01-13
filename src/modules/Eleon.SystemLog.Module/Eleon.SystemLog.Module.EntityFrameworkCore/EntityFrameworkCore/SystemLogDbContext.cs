using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.DocMessageLog.Module.Entities;
using SharedCollector.modules.Migration.Module.Extensions;

namespace VPortal.DocMessageLog.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public class SystemLogDbContext : AbpDbContext<SystemLogDbContext>, ISystemLogDbContext
{
  public DbSet<SystemLogEntity> SystemLogs { get; set; }

  public SystemLogDbContext(DbContextOptions<SystemLogDbContext> options)
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
