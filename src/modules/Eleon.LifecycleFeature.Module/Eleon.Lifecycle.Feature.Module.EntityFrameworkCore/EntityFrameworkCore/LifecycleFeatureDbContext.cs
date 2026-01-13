using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using SharedCollector.modules.Migration.Module.Extensions;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Entities.Conditions;

namespace VPortal.Lifecycle.Feature.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public class LifecycleFeatureDbContext : AbpDbContext<LifecycleFeatureDbContext>, ILifecycleFeatureDbContext
{
  public DbSet<StateActorAuditEntity> StateActorAuditEntity { get; set; }
  public DbSet<StateAuditEntity> StateAuditEntity { get; set; }
  public DbSet<ConditionEntity> Conditions { get; set; }
  public DbSet<StatesGroupAuditEntity> StatesGroupAudits { get; set; }
  public DbSet<StatesGroupTemplateEntity> StatesGroupTemplates { get; set; }

  public LifecycleFeatureDbContext(DbContextOptions<LifecycleFeatureDbContext> options)
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
