using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.LanguageManagement.Module.Entities;
using SharedCollector.modules.Migration.Module.Extensions;

namespace VPortal.LanguageManagement.Module.EntityFrameworkCore;

[ConnectionStringName(LanguageManagementDbProperties.ConnectionStringName)]
public class LanguageManagementDbContext : AbpDbContext<LanguageManagementDbContext>, ILanguageManagementDbContext
{
  public DbSet<LocalizationEntryEntity> LocalizationEntries { get; set; }
  public DbSet<LanguageEntity> Languages { get; set; }

  public LanguageManagementDbContext(DbContextOptions<LanguageManagementDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureLanguageManagement();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
