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
  public DbSet<EventQueueMessageEntity> EventQueueMessages { get; set; }
  public DbSet<EventQueueMessageBodyEntity> EventQueueMessageBodies { get; set; }

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

    builder.Entity<EventQueueMessageEntity>(b =>
    {
      b.HasKey(x => x.Id).IsClustered(false);

      b.Property(x => x.Lane).HasDefaultValue((byte)0);
      b.Property(x => x.EnqueueSeq).UseIdentityColumn();
      b.Property(x => x.Name).HasMaxLength(256).IsRequired();
      b.Property(x => x.Status).IsRequired();
      b.Property(x => x.Attempts).HasDefaultValue(0);
      b.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");
      b.Property(x => x.MessageKey).HasMaxLength(128);
      b.Property(x => x.TraceId).HasMaxLength(64);
      b.Property(x => x.LastError).HasMaxLength(1024);

      b.HasIndex(x => new { x.QueueId, x.Lane, x.Status, x.EnqueueSeq })
          .IsClustered()
          .IncludeProperties(x => new
          {
            x.Id,
            x.Name,
            x.TenantId,
            x.CreatedUtc,
            x.Attempts,
            x.VisibleAfterUtc,
            x.LockedUntilUtc,
            x.MessageKey,
            x.TraceId
          });

      b.HasIndex(x => new { x.QueueId, x.Lane, x.Status, x.LockedUntilUtc })
          .IncludeProperties(x => new { x.Id, x.EnqueueSeq });

      b.HasIndex(x => new { x.QueueId, x.Lane, x.MessageKey })
          .IsUnique()
          .HasFilter("[MessageKey] IS NOT NULL");

      b.HasOne(x => x.Body)
          .WithOne(x => x.Message)
          .HasForeignKey<EventQueueMessageBodyEntity>(x => x.Id)
          .OnDelete(DeleteBehavior.Cascade);
    });

    builder.Entity<EventQueueMessageBodyEntity>(b =>
    {
      b.HasKey(x => x.Id);
      b.Property(x => x.Payload).IsRequired();
      b.Property(x => x.ContentType).HasMaxLength(64).HasDefaultValue("application/json");
      b.Property(x => x.Encoding).HasMaxLength(32);
      b.HasQueryFilter(body => !IsMultiTenantFilterEnabled
          || (body.Message != null && body.Message.TenantId == CurrentTenantId));
    });

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
