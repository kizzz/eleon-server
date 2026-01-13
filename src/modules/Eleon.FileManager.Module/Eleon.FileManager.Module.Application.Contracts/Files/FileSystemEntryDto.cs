using Common.Module.Constants;
using System;
using VPortal.FileManager.Module.Constants;
using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.Files
{
  public class FileSystemEntryDto : FullAuditedEntityDto<string>
  {
    public EntryKind EntryKind { get; set; }
    public FileShareStatus SharedStatus { get; set; }
    public Guid ArchiveId { get; set; }
    public string Name { get; set; }
    public string? FolderId { get; set; }
    public string? ParentId { get; set; }
    public FileStatus Status { get; set; }
    public bool IsFavourite { get; set; }
    public string? Extension { get; set; }
    public string? Path { get; set; }
    public string? Size { get; set; }
    public string? ThumbnailPath { get; set; }
    public string? OriginalPath { get; set; }
    public string? OriginalThumbnailPath { get; set; }
    public byte[]? Source { get; set; }
    public string? LastModifierName { get; set; }

    // Folder-specific properties
    public string? PhysicalFolderId { get; set; }
    public bool IsShared { get; set; }

    // Navigation properties (for hierarchy)
    public FileSystemEntryDto? Parent { get; set; }
    public System.Collections.Generic.List<FileSystemEntryDto>? Children { get; set; }
    public System.Collections.Generic.List<FileSystemEntryDto>? Files { get; set; }
    public Guid? TenantId { get; set; }
  }
}



