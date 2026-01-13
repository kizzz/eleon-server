using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.FileManager.Module.Entities
{
  public class VirtualFolderEntity : FullAuditedAggregateRoot<string>, IMultiTenant
  {
    public string Name { get; set; }
    public string ParentId { get; set; }
    [NotMapped]
    public FileStatus Status { get; set; }
    [NotMapped]
    public VirtualFolderEntity? Parent { get; set; }
    public string Size { get; set; }
    public string PhysicalFolderId { get; set; }
    [NotMapped]
    public PhysicalFolderEntity PhysicalFolder { get; set; }
    [NotMapped]
    public string Path { get; set; }
    [NotMapped]
    public List<VirtualFolderEntity> Children { get; set; }
    [NotMapped]
    public List<FileEntity> Files { get; set; }
    public bool IsShared { get; set; }

    [NotMapped]
    public bool IsFavourite { get; set; }
    public Guid? TenantId { get; set; }

    protected VirtualFolderEntity() : base()
    {

    }
    public VirtualFolderEntity(string id) : base(id)
    {

    }
  }
}
