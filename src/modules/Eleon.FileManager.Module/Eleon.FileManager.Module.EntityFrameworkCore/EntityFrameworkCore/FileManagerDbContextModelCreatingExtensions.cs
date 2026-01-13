using Common.Module.Extensions;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;

namespace VPortal.FileManager.Module.EntityFrameworkCore;

public static class FileManagerDbContextModelCreatingExtensions
{
  public static void ConfigureModule(
      this ModelBuilder builder)
  {
    Check.NotNull(builder, nameof(builder));

    // Configure FileSystemEntry with TPH (Table Per Hierarchy)
    builder.Entity<FileSystemEntry>(b =>
    {
      b.ToTable("FileSystemEntries");

      // Common properties
      b.Property(e => e.Id).IsRequired().HasMaxLength(450);
      b.Property(e => e.Name).IsRequired().HasMaxLength(256);
      b.Property(e => e.ParentId).HasMaxLength(450);
      b.Property(e => e.ArchiveId).IsRequired();
      b.Property(e => e.EntryKind).IsRequired();
      b.Property(e => e.Size).HasMaxLength(50);
      b.Property(e => e.TenantId);

      // File-specific properties (nullable)
      b.Property(e => e.Extension).HasMaxLength(50);
      b.Property(e => e.Path).HasMaxLength(2000);
      b.Property(e => e.ThumbnailPath).HasMaxLength(2000);
      b.Property(e => e.FolderId).HasMaxLength(450);

      // Folder-specific properties
      b.Property(e => e.PhysicalFolderId).HasMaxLength(450);
      b.Property(e => e.IsShared);

      // Indexes
      b.HasIndex(e => e.ParentId);
      b.HasIndex(e => e.ArchiveId);
      b.HasIndex(e => e.EntryKind);
      b.HasIndex(e => new { e.ParentId, e.Name, e.EntryKind }); // For sibling uniqueness queries
    });
  }
}
