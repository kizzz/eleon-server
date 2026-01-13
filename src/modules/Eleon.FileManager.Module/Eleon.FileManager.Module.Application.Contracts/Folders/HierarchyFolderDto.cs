using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using VPortal.FileManager.Module.Files;

namespace VPortal.FileManager.Module.Folders
{
  public class HierarchyFolderDto : FullAuditedEntityDto<string>
  {
    public string Name { get; set; }
    public long SystemFolderName { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
    public Guid PhysicalFolderId { get; set; }
    public FileStatus Status { get; set; }
    public string Path { get; set; }
    public bool IsShared { get; set; }
    public List<HierarchyFolderDto> Children { get; set; } = new List<HierarchyFolderDto>();
    public List<FileDto> Files { get; set; } = new List<FileDto>();
  }
}
