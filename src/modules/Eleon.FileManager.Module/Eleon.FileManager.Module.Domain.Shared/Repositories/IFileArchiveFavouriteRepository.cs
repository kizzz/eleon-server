using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.FileManager.Module.Entities;

namespace VPortal.FileManager.Module.Repositories
{
  public interface IFileArchiveFavouriteRepository : IBasicRepository<FileArchiveFavouriteEntity, Guid>
  {
    Task<List<FileArchiveFavouriteEntity>> GetListAsync(Guid archiveId, string parentId, string userId);
    Task<FileArchiveFavouriteEntity> GetAsync(Guid archiveId, string fileId, string folderId, string userId);
  }
}
