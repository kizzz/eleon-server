using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.FileExternalLink
{
  public class FileExternalLinkDto : EntityDto<Guid>
  {
    public FileShareStatus PermissionType { get; set; }
    public Guid ArchiveId { get; set; }
    public Guid FileId { get; set; }
    public string WebUrl { get; set; }
    public string ExternalFileId { get; set; }
    public virtual List<FileExternalLinkReviewerDto> Reviewers { get; set; }
    public Guid? TenantId { get; set; }
  }
}
