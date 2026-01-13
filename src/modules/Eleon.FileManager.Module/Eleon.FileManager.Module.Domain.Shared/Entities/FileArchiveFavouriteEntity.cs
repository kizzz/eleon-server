using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.FileManager.Module.Entities
{
  public class FileArchiveFavouriteEntity : AggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public Guid ArchiveId { get; set; }
    public string ParentId { get; set; }
    public string UserId { get; set; }
    public string FileId { get; set; }
    public string FolderId { get; set; }
    protected FileArchiveFavouriteEntity()
        : base()
    {
    }

    public FileArchiveFavouriteEntity(Guid id, Guid archiveId, string fileId, string folderId, string parentId, string userId)
        : base(id)
    {
      ArchiveId = archiveId;
      ParentId = parentId;
      FileId = fileId;
      FolderId = folderId;
      UserId = userId;
    }
  }
}
