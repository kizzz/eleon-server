using Common.Module.Constants;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VPortal.FileManager.Module.Entities
{
  public class FileArchivePermissionTypeEntity : Entity<Guid>, IMultiTenant
  {
    public FileManagerPermissionType PermissionType { get; set; }

    public Guid? TenantId { get; set; }
    protected FileArchivePermissionTypeEntity() { }
    public FileArchivePermissionTypeEntity(Guid id, FileManagerPermissionType type)
        : base(id)
    {
      PermissionType = type;
    }
  }
}
