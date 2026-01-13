using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using VPortal.ExternalLink.Module.Entities;

namespace VPortal.ExternalLink.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public class ExternalLinkDbContext : AbpDbContext<ExternalLinkDbContext>, IExternalLinkDbContext
{
  public DbSet<ExternalLinkEntity> ExternalLinks { get; set; }

  public ExternalLinkDbContext(DbContextOptions<ExternalLinkDbContext> options)
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
