using EventManagementModule.Module.Domain.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Migrations.Module;
using SharedCollector.modules.Migration.Module.Extensions;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VPortal.EventManagementModule.Module.EntityFrameworkCore;

[ConnectionStringName(MigrationConsts.DefaultConnectionStringName)]
public class EventManagementModuleDbContext : AbpDbContext<EventManagementModuleDbContext>, IEventManagementModuleDbContext
{
  public DbSet<QueueDefinitionEntity> EventManagementQueueDefenition { get; set; }
  public DbSet<QueueEntity> EventManagementQueues { get; set; }
  public DbSet<EventEntity> EventManagementEvents { get; set; }

  //public DbSet<ForwarderEntity> EventManagementForwarders { get; set; }
  //public DbSet<QueueForwarderEntity> EventManagementQueueForwarderRelations { get; set; }

  public EventManagementModuleDbContext(DbContextOptions<EventManagementModuleDbContext> options)
      : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<QueueEntity>()
        //.HasIndex(q => new {q.TenantId, q.QueueDefinitionId})
        //.IsUnique()
        ;

    builder.Entity<QueueEntity>()
        .HasIndex(q => q.CreationTime);

    builder.Entity<QueueEntity>()
        .HasMany(q => q.Messages)
        .WithOne()
        .HasForeignKey(e => e.QueueId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Entity<EventEntity>()
        .HasIndex(m => m.QueueId);

    //builder.Entity<ForwarderEntity>()
    //    .HasIndex(m => m.TenantId);

    //builder.Entity<ForwarderEntity>()
    //    .HasMany(f => f.Queues)
    //    .WithOne(f => f.Forwarder)
    //    .HasForeignKey(qf => qf.ForwarderId)
    //    .OnDelete(DeleteBehavior.Cascade);

    //builder.Entity<QueueForwarderEntity>()
    //    .HasOne(qf => qf.Queue)
    //    .WithMany()
    //    .HasForeignKey(qf => qf.QueueId);
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
