using Eleon.Templating.Module.Templates;
using Microsoft.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Eleon.Templating.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public class TemplatingDbContext : AbpDbContext<TemplatingDbContext>, ITemplatingDbContext
{
  public DbSet<Template> Templates { get; set; } = default!;

  public TemplatingDbContext(DbContextOptions<TemplatingDbContext> options)
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
