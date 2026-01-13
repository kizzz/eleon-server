using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using VPortal.FileManager.Module.Constants;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.FileManager.Module.Entities
{
  public class FileSystemEntry : FullAuditedAggregateRoot<string>, IMultiTenant
  {
    // Common fields
    public Guid ArchiveId { get; set; }
    public string Name { get; set; }
    public string? ParentId { get; set; }
    public EntryKind EntryKind { get; set; }
    public Guid? TenantId { get; set; }
    public string Size { get; set; }

    // File-specific fields (nullable, guarded by domain rules)
    public string? Extension { get; set; }
    public string? Path { get; set; }
    public string? ThumbnailPath { get; set; }
    public string? FolderId { get; set; } // Legacy, maps to ParentId

    // Folder-specific fields
    public string? PhysicalFolderId { get; set; }
    public bool IsShared { get; set; }

    // NotMapped properties
    [NotMapped]
    public FileStatus Status { get; set; }

    [NotMapped]
    public bool IsFavourite { get; set; }

    [NotMapped]
    public FileShareStatus SharedStatus { get; set; } = FileShareStatus.None;

    [NotMapped]
    public string? LastModifierName { get; set; }

    [NotMapped]
    public string? OriginalPath { get; set; }

    [NotMapped]
    public string? OriginalThumbnailPath { get; set; }

    [NotMapped]
    public byte[]? Source { get; set; }

    [NotMapped]
    public FileSystemEntry? Parent { get; set; }

    [NotMapped]
    public List<FileSystemEntry>? Children { get; set; }

    [NotMapped]
    public List<FileSystemEntry>? Files { get; set; }

    protected FileSystemEntry() : base()
    {
    }

    public FileSystemEntry(string id) : base(id)
    {
    }

    public FileSystemEntry(FileSystemEntry oldEntity, string id) : base(id)
    {
      ArchiveId = oldEntity.ArchiveId;
      Name = oldEntity.Name;
      FolderId = oldEntity.FolderId;
      Extension = oldEntity.Extension;
      Path = oldEntity.Path;
      Size = oldEntity.Size;
      ParentId = oldEntity.Id;
      EntryKind = oldEntity.EntryKind;
      LastModificationTime = DateTime.Now;
      ThumbnailPath = oldEntity.ThumbnailPath;
      TenantId = oldEntity.TenantId;
      PhysicalFolderId = oldEntity.PhysicalFolderId;
      IsShared = oldEntity.IsShared;
    }

    public static FileSystemEntry CreateFile(string id, Guid archiveId, string name, string? parentId, string? extension = null, string? path = null, string? size = null, string? thumbnailPath = null)
    {
      return new FileSystemEntry(id)
      {
        ArchiveId = archiveId,
        Name = name,
        ParentId = parentId,
        EntryKind = EntryKind.File,
        Extension = extension,
        Path = path,
        Size = size ?? "0",
        ThumbnailPath = thumbnailPath,
        FolderId = parentId
      };
    }

    public static FileSystemEntry CreateFolder(string id, Guid archiveId, string name, string? parentId, string? physicalFolderId = null, bool isShared = false, string? size = null)
    {
      return new FileSystemEntry(id)
      {
        ArchiveId = archiveId,
        Name = name,
        ParentId = parentId,
        EntryKind = EntryKind.Folder,
        PhysicalFolderId = physicalFolderId,
        IsShared = isShared,
        Size = size ?? "0"
      };
    }
  }
}



