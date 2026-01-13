using Common.Module.Constants;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;

namespace VPortal.FileManager.Module.DomainServices
{
  public class FileStatusDomainService : DomainService
  {
    private readonly IVportalLogger<FileStatusDomainService> logger;
    private readonly IFileStatusRepository fileStatusRepository;

    public FileStatusDomainService(
        IVportalLogger<FileStatusDomainService> logger,
        IFileStatusRepository fileStatusRepository)
    {
      this.logger = logger;
      this.fileStatusRepository = fileStatusRepository;
    }

    public async Task<FileStatus> GetFileStatusAsync(Guid archiveId, string fileId)
    {

      FileStatus result = FileStatus.Active;
      try
      {
        var status = await fileStatusRepository
            .GetByFileId(archiveId, fileId);
        if (status != null)
        {
          result = status.FileStatus;
        }
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
    public async Task<FileStatus> GetFolderStatusAsync(Guid archiveId, string folderId)
    {

      FileStatus result = FileStatus.Active;
      try
      {
        var status = await fileStatusRepository
            .GetByFolderId(archiveId, folderId);

        result = status.FileStatus;
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

      List<FileStatusEntity> result = new();
      try
      {
        result = await fileStatusRepository.GetFileListAsync(archiveId, fileStatuses);
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
    public async Task<List<FileStatusEntity>> GetFolderListAsync(Guid archiveId, List<FileStatus> fileStatuses)
    {

      List<FileStatusEntity> result = new List<FileStatusEntity>();
      try
      {
        result = await fileStatusRepository.GetFolderListAsync(archiveId, fileStatuses);
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
    public async Task<bool> FillStatuses(Guid archiveId, List<FileSystemEntry> files, List<FileSystemEntry> folders)
    {


      bool result = false;
      try
      {
        var dbSet = await fileStatusRepository.GetDbSetAsync();

        foreach (var file in files)
        {
          var status = dbSet.FirstOrDefault(f => f.FileId == file.Id && f.ArchiveId == archiveId);

          if (status != null)
          {
            file.Status = status.FileStatus;
          }
          else
          {
            file.Status = FileStatus.Active;
          }
        }
        foreach (var folder in folders)
        {
          var status = dbSet.FirstOrDefault(f => f.FolderId == folder.Id && f.ArchiveId == archiveId);

          if (status != null)
          {
            folder.Status = status.FileStatus;
          }
          else
          {
            folder.Status = FileStatus.Active;
          }
        }

        result = true;
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
    public async Task<bool> UpdateFileStatus(Guid archiveId, string fileId, FileStatus newStatus)
    {
      bool result = false;
      try
      {
        result = await fileStatusRepository.UpdateFileStatus(archiveId, fileId, newStatus);
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
    public async Task<bool> UpdateFolderStatus(Guid archiveId, string folderId, FileStatus newStatus)
    {
      bool result = false;
      try
      {
        result = await fileStatusRepository.UpdateFolderStatus(archiveId, folderId, newStatus);
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
