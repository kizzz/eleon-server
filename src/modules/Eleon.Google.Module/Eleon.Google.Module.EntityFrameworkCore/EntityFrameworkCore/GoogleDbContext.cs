using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.Google.Module.EntityFrameworkCore;

[ConnectionStringName(GoogleDbProperties.ConnectionStringName)]
public class GoogleDbContext : AbpDbContext<GoogleDbContext>, IGoogleDbContext
{
  /* Add DbSet for each Aggregate Root here. Example:
   * public DbSet<Question> Questions { get; set; }
   */

  public GoogleDbContext(DbContextOptions<GoogleDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureGoogle();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
