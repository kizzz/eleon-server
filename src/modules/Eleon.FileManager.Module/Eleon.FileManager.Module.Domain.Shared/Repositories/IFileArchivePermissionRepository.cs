using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Repositories
{
  public interface IFileArchivePermissionRepository : IBasicRepository<FileArchivePermissionEntity, Guid>
  {
    Task<List<FileArchivePermissionEntity>> GetListAsync(Guid archiveId, List<string> folderId);
    Task<FileArchivePermissionEntity> GetPermissionWithoutDefault(FileArchivePermissionKeyValueObject keyValueObject);
    Task<FileArchivePermissionEntity> UpdatePermissions(FileArchivePermissionValueObject valueObject);
    Task<bool> DeletePermissions(FileArchivePermissionValueObject permissionValueObject);
  }
}
