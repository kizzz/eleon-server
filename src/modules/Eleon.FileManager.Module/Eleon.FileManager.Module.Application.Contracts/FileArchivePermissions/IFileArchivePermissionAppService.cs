using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.FileManager.Module.FileArchivePermissions
{
  public interface IFileArchivePermissionAppService : IApplicationService
  {
    public Task<FileArchivePermissionDto> UpdatePermission(FileArchivePermissionDto updatedPermissionDto);
    public Task<FileArchivePermissionDto> GetPermissionWithoutDefault(FileArchivePermissionKeyDto key);
    public Task<IEnumerable<FileManagerPermissionType>> GetPermissionOrDefault(FileArchivePermissionKeyDto key);
    public Task<List<FileArchivePermissionDto>> GetList(Guid archiveId, string folderId);
    Task<bool> DeletePermissions(FileArchivePermissionDto deletedPermissionDto);
  }
}
