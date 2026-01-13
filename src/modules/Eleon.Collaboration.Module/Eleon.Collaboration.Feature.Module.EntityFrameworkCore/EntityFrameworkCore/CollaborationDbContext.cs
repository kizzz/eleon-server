using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Collaboration.Feature.Module.Entities;
using SharedCollector.modules.Migration.Module.Extensions;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Entities;

namespace VPortal.Collaboration.Feature.Module.EntityFrameworkCore;

[ConnectionStringName(CollaborationDbProperties.ConnectionStringName)]
public class CollaborationDbContext : AbpDbContext<CollaborationDbContext>, ICollaborationDbContext
{
  public DbSet<ChatMemberEntity> ChatMembers { get; set; }
  public DbSet<ChatMessageEntity> ChatMessages { get; set; }
  public DbSet<ChatRoomEntity> ChatRooms { get; set; }
  public DbSet<UserChatSettingEntity> UserChatSettings { get; set; }
  public DbSet<ViewChatPermissionEntity> ViewChatPermissions { get; set; }

  public CollaborationDbContext(DbContextOptions<CollaborationDbContext> options)
      : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ConfigureChat();
    builder.ConfigureEntitiesWithPrefix(this, "Ec");
  }
}
