using Common.Module.Constants;
using ModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Entities
{
  public class FileArchivePermissionEntity : AggregateRoot<Guid>, IMultiTenant
  {
    public Guid ArchiveId { get; set; }
    public string FolderId { get; set; }
    public PermissionActorType ActorType { get; set; }
    public string ActorId { get; set; }
    [NotMapped]
    public string ActorDisplayName { get; set; }
    public List<FileArchivePermissionTypeEntity> PermissionTypes { get; set; }
    public Guid? TenantId { get; set; }
    [NotMapped]
    public IEnumerable<FileManagerPermissionType> AllowedPermissions
    {
      get
      {
        return this.PermissionTypes.Select(p => p.PermissionType);
      }
    }
    [NotMapped]
    public string UniqueKey { get { return ActorType.ToString() + ActorId; } }

    protected FileArchivePermissionEntity()
    {

    }

    public FileArchivePermissionEntity(FileArchivePermissionKeyValueObject permissionValueObject)
    {
      ArchiveId = permissionValueObject.ArchiveId;
      FolderId = permissionValueObject.FolderId;
      ActorType = permissionValueObject.ActorType;
      ActorId = permissionValueObject.ActorId;
      PermissionTypes = new List<FileArchivePermissionTypeEntity>();
    }
  }
}
