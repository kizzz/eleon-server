using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using VPortal.Vpn;

namespace VPortal.EntityFrameworkCore;

[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public class VPortalDbContext : VPortalDbContextBase<VPortalDbContext>
{
  public DbSet<Tenant> Tenants { get; set; }
  public DbSet<VpnServerSettingsEntity> VpnServerSettings { get; set; }

  public VPortalDbContext(DbContextOptions<VPortalDbContext> options)
      : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.SetMultiTenancySide(MultiTenancySides.Both);

    base.OnModelCreating(builder);
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
