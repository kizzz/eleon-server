using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.FileManager.Module.Entities;

namespace VPortal.FileManager.Module.Repositories
{
  public interface IFileStatusRepository : IBasicRepository<FileStatusEntity, Guid>
  {
    public Task<List<FileStatusEntity>> GetFileListAsync(Guid archiveId, List<FileStatus> fileStatuses);
    public Task<List<FileStatusEntity>> GetFolderListAsync(Guid archiveId, List<FileStatus> folderStatuses);
    public Task<bool> UpdateFolderStatus(Guid archiveId, string folderId, FileStatus folderStatus);
    public Task<bool> UpdateFileStatus(Guid archiveId, string fileId, FileStatus fileStatus);
    public Task<FileStatusEntity> GetByFileId(Guid archiveId, string fileId);
    public Task<FileStatusEntity> GetByFolderId(Guid archiveId, string folderId);
  }
}
