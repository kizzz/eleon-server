using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module.EntityFrameworkCore;

public static class CollaborationDbContextModelCreatingExtensions
{
  public static void ConfigureChat(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));

    builder
        .Entity<ChatRoomEntity>()
        .HasMany(x => x.Messages)
        .WithOne(x => x.ChatRoom)
        .HasForeignKey(x => x.ChatRoomId)
        .OnDelete(DeleteBehavior.Cascade);

    builder
        .Entity<ChatRoomEntity>()
        .HasMany(x => x.ViewChatPermissions)
        .WithOne(x => x.Chat)
        .HasForeignKey(x => x.ChatId)
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired(false);
  }
}
