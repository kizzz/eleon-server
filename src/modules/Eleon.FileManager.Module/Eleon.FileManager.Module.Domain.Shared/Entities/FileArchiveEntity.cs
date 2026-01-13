using Common.Module.Constants;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.FileManager.Module.Entities
{
  public class FileArchiveEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public string Name { get; set; }
    public FileArchiveHierarchyType FileArchiveHierarchyType { get; set; }
    public Guid StorageProviderId { get; set; }
    public Guid FileEditStorageProviderId { get; set; }
    [NotMapped]
    public string StorageProviderName { get; set; }
    public string RootFolderId { get; set; }
    public string PhysicalRootFolderId { get; set; }
    public Guid? TenantId { get; set; }
    public bool IsActive { get; set; }
    public bool IsPersonalizedArchive { get; set; }

    protected FileArchiveEntity() { }

    public FileArchiveEntity(Guid id)
        : base(id)
    {
    }
  }
}
