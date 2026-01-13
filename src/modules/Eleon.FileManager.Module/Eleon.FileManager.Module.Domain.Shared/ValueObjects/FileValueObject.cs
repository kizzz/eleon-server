using Common.Module.Constants;
using System;

namespace VPortal.FileManager.Module.ValueObjects
{
  public class FileValueObject
  {
    public Guid Id { get; set; }
    public FileShareStatus SharedStatus { get; set; } = FileShareStatus.None;
    public Guid ArchiveId { get; set; }
    public string Name { get; set; }
    public string FolderId { get; set; }
    public string Extension { get; set; }
    public string Path { get; set; }
    public string Size { get; set; }
    public string ThumbnailPath { get; set; }
    public bool IsFavourite { get; set; }
    public string OriginalPath { get; set; }
    public string OriginalThumbnailPath { get; set; }
    public byte[] Source { get; set; }
    public string ParentId { get; set; }
  }
}
