using Core.Infrastructure.Module.Entities;
using Microsoft.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Core.Infrastructure.Module.Entities;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Infrastructure.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public class InfrastructureDbContext : AbpDbContext<InfrastructureDbContext>, IInfrastructureDbContext
{
  //public DbSet<UnitPermissionEntity> UnitPermissions { get; }
  //public DbSet<CompanyEntity> Companies { get; }
  public DbSet<SeriaNumberEntity> SeriaNumbers { get; set; }
  public DbSet<AddressEntity> Addresses { get; set; }
  public DbSet<CountryEntity> Countries { get; set; }
  public DbSet<DashboardSettingEntity> DashboardSettings { get; set; }

  public DbSet<FeatureSettingEntity> FeatureSettings { get; set; }

  public InfrastructureDbContext(DbContextOptions<InfrastructureDbContext> options)
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
