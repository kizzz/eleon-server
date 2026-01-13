using Common.Module.Constants;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.FileManager.Module.Entities
{
  public class FileStatusEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid ArchiveId { get; set; }
    public string FileId { get; set; }
    public string FolderId { get; set; }
    public FileStatus FileStatus { get; set; }
    public DateTime StatusDate { get; set; }
    public Guid? TenantId { get; set; }

    protected FileStatusEntity() : base()
    {

    }

    public FileStatusEntity(Guid id, Guid archiveId, string fileId, string folderId, FileStatus fileStatus, DateTime statusDate) : base(id)
    {
      ArchiveId = archiveId;
      FileId = fileId;
      FolderId = folderId;
      FileStatus = fileStatus;
      StatusDate = statusDate;
    }

  }
}
