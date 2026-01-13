using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Helpers.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.EntityFrameworkCore;

namespace VPortal.FileManager.Module.Repositories
{
  internal class FileStatusRepository :
      EfCoreRepository<FileManagerDbContext, FileStatusEntity, Guid>,
      IFileStatusRepository
  {
    private readonly IVportalLogger<FileStatusRepository> logger;
    private readonly IDbContextProvider<FileManagerDbContext> dbContextProvider;

    public FileStatusRepository(
        IVportalLogger<FileStatusRepository> logger,
        IDbContextProvider<FileManagerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public async Task<FileStatusEntity> GetByFileId(Guid archiveId, string fileId)
    {

      FileStatusEntity result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet.FirstOrDefaultAsync(f => f.ArchiveId == archiveId && f.FileId == fileId);
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

    public async Task<FileStatusEntity> GetByFolderId(Guid archiveId, string folderId)
    {
      FileStatusEntity result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = await dbSet.FirstOrDefaultAsync(f => f.ArchiveId == archiveId && f.FolderId == folderId);
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

    public async Task<List<FileStatusEntity>> GetFileListAsync(Guid archiveId, List<FileStatus> fileStatuses)
    {
      List<FileStatusEntity> result = new List<FileStatusEntity>();
      try
      {
        var dbSet = await GetDbSetAsync();
        var entities = await dbSet.Where(f => f.ArchiveId == archiveId)
            .Where(f => fileStatuses.Contains(f.FileStatus))
            .ToListAsync();

        result = entities
            .Where(f => !f.FileId.IsNullOrEmpty())
            .ToList();
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

    public async Task<List<FileStatusEntity>> GetFolderListAsync(Guid archiveId, List<FileStatus> folderStatuses)
    {
      List<FileStatusEntity> result = new List<FileStatusEntity>();
      try
      {
        var dbSet = await GetDbSetAsync();
        var entities = await dbSet.Where(f => f.ArchiveId == archiveId)
            .Where(f => folderStatuses.Contains(f.FileStatus))
            .ToListAsync();

        result = entities
            .Where(f => !f.FileId.IsNullOrEmpty())
            .ToList();
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

    public async Task<bool> UpdateFileStatus(Guid archiveId, string fileId, FileStatus fileStatus)
    {
      bool result = false;
      try
      {
        var dbSet = await GetDbSetAsync();

        var status = dbSet.FirstOrDefault(f => f.FileId == fileId && f.ArchiveId == archiveId);

        if (status != null)
        {
          // Idempotency: if already in desired status, treat as success
          if (status.FileStatus == fileStatus)
          {
            logger.Log.LogInformation(
                "File status for file {FileId} in archive {ArchiveId} already in desired status ({Status}). Treating as idempotent success.",
                fileId, archiveId, fileStatus);
            return true;
          }

          status.FileStatus = fileStatus;

          try
          {
            await UpdateAsync(status, true);
            result = true;
          }
          catch (AbpDbConcurrencyException ex)
          {
            logger.Log.LogWarning(
                ex,
                "Concurrency conflict while updating file status for file {FileId} in archive {ArchiveId}. Waiting for desired state...",
                fileId, archiveId);

            var resolved = await ConcurrencyExtensions.WaitForDesiredStateAsync(
              async () =>
              {
                var currentStatus = await dbSet.FirstOrDefaultAsync(f => f.FileId == fileId && f.ArchiveId == archiveId);
                var isDesired = currentStatus != null && currentStatus.FileStatus == fileStatus;
                var details = currentStatus == null
                  ? "FileStatusEntity not found"
                  : $"Status={currentStatus.FileStatus}";
                return new ConcurrencyExtensions.ConcurrencyWaitResult<FileStatusEntity>(isDesired, currentStatus, details);
              },
              logger.Log,
              "UpdateFileStatus",
              $"{archiveId}/{fileId}"
            );

            result = resolved != null;
          }
          return result;
        }

        status = new Entities.FileStatusEntity(GuidGenerator.Create(), archiveId, fileId, null, fileStatus, DateTime.Now);
        await InsertAsync(status, true);
        result = true;
      }
      catch (AbpDbConcurrencyException)
      {
        throw; // Re-throw after handling above
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

    public async Task<bool> UpdateFolderStatus(Guid archiveId, string folderId, FileStatus folderStatus)
    {
      bool result = false;
      try
      {
        var dbSet = await GetDbSetAsync();

        var status = dbSet.FirstOrDefault(f => f.FolderId == folderId && f.ArchiveId == archiveId);

        if (status != null)
        {
          // Idempotency: if already in desired status, treat as success
          if (status.FileStatus == folderStatus)
          {
            logger.Log.LogInformation(
                "Folder status for folder {FolderId} in archive {ArchiveId} already in desired status ({Status}). Treating as idempotent success.",
                folderId, archiveId, folderStatus);
            return true;
          }

          status.FileStatus = folderStatus;

          try
          {
            await UpdateAsync(status, true);
            result = true;
          }
          catch (AbpDbConcurrencyException ex)
          {
            logger.Log.LogWarning(
                ex,
                "Concurrency conflict while updating folder status for folder {FolderId} in archive {ArchiveId}. Waiting for desired state...",
                folderId, archiveId);

            var resolved = await ConcurrencyExtensions.WaitForDesiredStateAsync(
              async () =>
              {
                var currentStatus = await dbSet.FirstOrDefaultAsync(f => f.FolderId == folderId && f.ArchiveId == archiveId);
                var isDesired = currentStatus != null && currentStatus.FileStatus == folderStatus;
                var details = currentStatus == null
                  ? "FileStatusEntity not found"
                  : $"Status={currentStatus.FileStatus}";
                return new ConcurrencyExtensions.ConcurrencyWaitResult<FileStatusEntity>(isDesired, currentStatus, details);
              },
              logger.Log,
              "UpdateFolderStatus",
              $"{archiveId}/{folderId}"
            );

            result = resolved != null;
          }
          return result;
        }

        status = new Entities.FileStatusEntity(GuidGenerator.Create(), archiveId, null, folderId, folderStatus, DateTime.Now);

        await InsertAsync(status, true);
        result = true;
      }
      catch (AbpDbConcurrencyException)
      {
        throw; // Re-throw after handling above
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
