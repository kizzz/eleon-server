using System;
using System.Linq.Expressions;
using Volo.Abp.Specifications;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Specifications
{
  public class FileArchivePermissionKeyMatchSpecification : Specification<FileArchivePermissionEntity>
  {
    private readonly FileArchivePermissionKeyValueObject keyValueObject;

    public FileArchivePermissionKeyMatchSpecification(FileArchivePermissionKeyValueObject keyValueObject)
    {
      this.keyValueObject = keyValueObject;
    }

    public override Expression<Func<FileArchivePermissionEntity, bool>> ToExpression()
    {
      return (permissionEntity) => permissionEntity.ArchiveId == keyValueObject.ArchiveId
          && permissionEntity.FolderId == keyValueObject.FolderId
          && permissionEntity.ActorType == keyValueObject.ActorType
          && permissionEntity.ActorId == keyValueObject.ActorId;
    }
  }
}
