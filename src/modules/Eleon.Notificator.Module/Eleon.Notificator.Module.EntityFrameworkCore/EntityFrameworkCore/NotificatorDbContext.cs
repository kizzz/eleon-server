using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Notificator.Module.Entities;
using SharedCollector.modules.Migration.Module.Extensions;

namespace VPortal.Notificator.Module.EntityFrameworkCore;

[ConnectionStringName(ModuleDbProperties.ConnectionStringName)]
public class NotificatorDbContext : AbpDbContext<NotificatorDbContext>, INotificatorDbContext
{
  public DbSet<NotificationEntity> Notifications { get; set; }
  public DbSet<NotificationLogEntity> NotificationLogs { get; set; }
  public DbSet<WebPushSubscriptionEntity> WebPushes { get; set; }
  public DbSet<UserNotificationSettingsEntity> UserNotificationSettings { get; set; }

  public NotificatorDbContext(DbContextOptions<NotificatorDbContext> options)
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
