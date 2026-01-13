using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.PhysicalFolders;

namespace VPortal.FileManager.Module.Folders
{
  public class VirtualFolderDto : FullAuditedEntityDto<string>
  {
    public string Name { get; set; }
    public string ParentId { get; set; }
    public VirtualFolderDto? Parent { get; set; }
    public string Size { get; set; }
    public FileStatus Status { get; set; }
    public string PhysicalFolderId { get; set; }
    public PhysicalFolderDto? PhysicalFolder { get; set; }
    public string Path { get; set; }
    public List<VirtualFolderDto> Children { get; set; }
    public List<FileDto> Files { get; set; }
    public bool IsShared { get; set; }
    public bool IsFavourite { get; set; }
    public Guid? TenantId { get; set; }
  }
}
