using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;

namespace VPortal.ApplicationConfiguration.Module.EntityFrameworkCore;

/// <summary>
/// Example DbContext for ApplicationConfiguration module
/// </summary>
[ConnectionStringName(ApplicationConfigurationDbProperties.ConnectionStringName)]
public class ApplicationConfigurationDbContext : AbpDbContext<ApplicationConfigurationDbContext>, IApplicationConfigurationDbContext
{

  public ApplicationConfigurationDbContext(DbContextOptions<ApplicationConfigurationDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureApplicationConfiguration();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
