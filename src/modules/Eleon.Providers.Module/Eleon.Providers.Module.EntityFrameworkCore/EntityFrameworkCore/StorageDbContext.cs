using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using VPortal.Storage.Module.Entities;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Entities;

namespace VPortal.Storage.Module.EntityFrameworkCore;

[ConnectionStringName(ProviderModuleDbProperties.ConnectionStringName)]
public class StorageDbContext : AbpDbContext<StorageDbContext>, IStorageDbContext
{
  public DbSet<StorageProviderEntity> StorageProviders { get; set; }
  public DbSet<StorageProviderSettingEntity> StorageProviderSettings { get; set; }
  public DbSet<StorageProviderSettingTypeEntity> StorageProviderSettingTypes { get; set; }
  public DbSet<StorageProviderTypeEntity> StorageProviderTypes { get; set; }

  public StorageDbContext(DbContextOptions<StorageDbContext> options)
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
