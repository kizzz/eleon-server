using Common.Module.Constants;
using System;
using Volo.Abp.Application.Dtos;

namespace VPortal.FileManager.Module.FileArchives
{
  public class FileArchiveDto : EntityDto<Guid>
  {
    public string Name { get; set; }
    public string RootFolderId { get; set; }
    public string PhysicalRootFolderId { get; set; }
    public FileArchiveHierarchyType FileArchiveHierarchyType { get; set; }
    public Guid StorageProviderId { get; set; }
    public Guid FileEditStorageProviderId { get; set; }
    public string StorageProviderName { get; set; }
    public bool IsActive { get; set; }
    public bool IsPersonalizedArchive { get; set; }
    //public DateTime CreationTime { get; set; }
  }
}
