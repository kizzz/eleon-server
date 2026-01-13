using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.EntityFrameworkCore;

namespace VPortal.FileManager.Module.Repositories
{

  public class FileArchiveFavouriteRepository :
      EfCoreRepository<FileManagerDbContext, FileArchiveFavouriteEntity, Guid>,
      IFileArchiveFavouriteRepository
  {
    private readonly IVportalLogger<FileArchiveFavouriteRepository> logger;
    private readonly IDbContextProvider<FileManagerDbContext> dbContextProvider;

    public FileArchiveFavouriteRepository(
        IVportalLogger<FileArchiveFavouriteRepository> logger,
        IDbContextProvider<FileManagerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }


    public async Task<FileArchiveFavouriteEntity> GetAsync(Guid archiveId, string fileId, string folderId, string userId)
    {
      FileArchiveFavouriteEntity result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet
            .FirstOrDefaultAsync(c => c.ArchiveId == archiveId && c.FileId == fileId && c.FolderId == folderId && c.UserId == userId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<FileArchiveFavouriteEntity>> GetListAsync(Guid archiveId, string parentId, string userId)
    {
      List<FileArchiveFavouriteEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet
            .Where(c => c.UserId == userId)
            .Where(c => c.ArchiveId == archiveId)
            .WhereIf(parentId != null, c => parentId == c.ParentId)
            .ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
  }
}
