using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.SystemServicesModule.Module.EntityFrameworkCore;

[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public class SystemServicesModuleDbContext : AbpDbContext<SystemServicesModuleDbContext>, ISystemServicesModuleDbContext
{
  public SystemServicesModuleDbContext(DbContextOptions<SystemServicesModuleDbContext> options)
      : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}

