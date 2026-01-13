using Common.Module.Constants;
using System;
using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.Files
{
  public class FileDto : FullAuditedEntityDto<string>
  {
    public FileShareStatus SharedStatus { get; set; }
    public Guid ArchiveId { get; set; }
    public string Name { get; set; }
    public string FolderId { get; set; }
    public FileStatus Status { get; set; }
    public bool IsFavourite { get; set; }
    //public PhysicalFolderDto PhysicalFolder { get; set; }
    public string Extension { get; set; }
    public string Path { get; set; }
    public string Size { get; set; }
    public string ThumbnailPath { get; set; }
    public string OriginalPath { get; set; }
    public string OriginalThumbnailPath { get; set; }
    public byte[] Source { get; set; }
    public string LastModifierName { get; set; }
  }
}
