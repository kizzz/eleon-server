using Common.Module.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.FileManager.Module.Entities
{
  public class FileEntity : FullAuditedAggregateRoot<string>, IMultiTenant
  {
    public Guid ArchiveId { get; set; }
    public string Name { get; set; }
    public string FolderId { get; set; }
    [NotMapped]
    public FileStatus Status { get; set; }
    // public PhysicalFolderEntity PhysicalFolder {get; set;}
    public string Extension { get; set; }
    public string Path { get; set; }
    public string Size { get; set; }
    public string ParentId { get; set; }
    public string ThumbnailPath { get; set; }
    [NotMapped]
    public string OriginalPath { get; set; }
    [NotMapped]
    public string OriginalThumbnailPath { get; set; }
    [NotMapped]
    public byte[] Source { get; set; }
    public Guid? TenantId { get; set; }

    [NotMapped]
    public bool IsFavourite { get; set; }
    [NotMapped]
    public FileShareStatus SharedStatus { get; set; } = FileShareStatus.None;

    [NotMapped]
    public string LastModifierName { get; set; }

    protected FileEntity() : base()
    {

    }
    public FileEntity(string id) : base(id)
    {

    }
    public FileEntity(FileEntity oldEntity, string id) : base(id)
    {
      ArchiveId = oldEntity.ArchiveId;
      Name = oldEntity.Name;
      FolderId = oldEntity.FolderId;
      Extension = oldEntity.Extension;
      Path = oldEntity.Path;
      Size = oldEntity.Size;
      ParentId = oldEntity.Id;
      LastModificationTime = DateTime.Now;
      ThumbnailPath = oldEntity.ThumbnailPath;
      TenantId = oldEntity.TenantId;
    }
  }
}
