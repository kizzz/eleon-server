using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace VPortal.EntityFrameworkCore;

[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public class VPortalTenantDbContext : VPortalDbContextBase<VPortalTenantDbContext>
{
  public VPortalTenantDbContext(DbContextOptions<VPortalTenantDbContext> options)
      : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.SetMultiTenancySide(MultiTenancySides.Tenant);

    base.OnModelCreating(builder);
  }
}
