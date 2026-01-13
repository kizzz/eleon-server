using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.FileManager.Module.Entities
{
  public class FileExternalLinkEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public FileShareStatus PermissionType { get; set; }
    public Guid ArchiveId { get; set; }
    public string FileId { get; set; }
    public string WebUrl { get; set; }
    public string ExternalFileId { get; set; }
    public virtual List<FileExternalLinkReviewerEntity> Reviewers { get; set; }
    public Guid? TenantId { get; set; }

    public FileExternalLinkEntity()
    {
      Reviewers = new List<FileExternalLinkReviewerEntity>();
    }
    public FileExternalLinkEntity(Guid id) : base(id)
    {
      Reviewers = new List<FileExternalLinkReviewerEntity>();
    }
  }
}
