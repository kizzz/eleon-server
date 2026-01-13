using FileManager.Remote.Application.Contracts.Files;
using Logging.Module;
using System;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.ValueObjects;

namespace FileManager.Remote.Application.Files
{
  public class FileRemoteAppService : IFileRemoteAppService
  {
    private readonly IVportalLogger<FileRemoteAppService> logger;
    private readonly IFileFactory factory;

    public FileArchiveEntity Archive { get; set; }

    public FileRemoteAppService(
        IVportalLogger<FileRemoteAppService> logger,
        IFileFactory factory)
    {
      this.logger = logger;
      this.factory = factory;
    }

    public async Task<List<FileSystemEntry>> GetFilesByFolderId(string id)
    {
      List<FileSystemEntry> result = null;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.GetEntriesByParentId(id, VPortal.FileManager.Module.Constants.EntryKind.File);
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

    public async Task<string> GetFileToken(string id, bool isVersion)
    {
      string result = string.Empty;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.GetFileToken(id, isVersion);
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

    public async Task<bool> DeleteFileToken(Guid token)
    {
      bool result = false;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.DeleteFileToken(token);
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

    public async Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion)
    {
      byte[] result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.GetFileByToken(id, token, isVersion);
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

    public async Task<bool> MoveFile(string fileId, string destinationFolderId)
    {
      bool result = false;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.MoveEntry(fileId, destinationFolderId);
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

    public async Task<bool> MoveAllFile(List<string> fileIds, List<string> folders, string destinationFolderId)
    {
      bool result = false;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        var allEntryIds = new List<string>(fileIds);
        allEntryIds.AddRange(folders);
        result = await repository.MoveAllEntries(allEntryIds, destinationFolderId);
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

    public async Task<FileSourceValueObject> FileViewer(string id)
    {
      FileSourceValueObject result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.FileViewer(id);
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

    public async Task<bool> CopyFile(string fileId, string destinationFolderId)
    {
      bool result = false;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.CopyEntry(fileId, destinationFolderId);
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


    public async Task<bool> DeleteFile(string id)
    {
      bool result = false;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.DeleteEntry(id);
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

    public async Task<byte[]> DownloadFile(string id, bool isVersion)
    {
      byte[] result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.DownloadFile(id, isVersion);
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

    public async Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion)
    {
      byte[] result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.DownloadFileByToken(id, token, isVersion);
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

    public async Task<List<string>> ReadTextFile(string id, bool isVersion)
    {
      List<string> result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.ReadTextFile(id, isVersion);
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

    public async Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion)
    {
      List<string> result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.ReadTextFileByToken(id, token, isVersion);
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

    public async Task<List<FileSystemEntry>> SearchFile(string searchString)
    {
      List<FileSystemEntry> result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.SearchEntries(searchString, EntryKind.File);
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

    public async Task<bool> RenameFile(string id, string name)
    {
      bool result = false;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        var entry = await repository.RenameEntry(id, name);
        result = entry != null;
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

    public async Task<FileSystemEntry> CreateFolder(string name, string folderId, string parentId)
    {
      FileSystemEntry result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.CreateEntry(EntryKind.Folder, name, parentId, folderId);
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

    public async Task<bool> DeleteFolder(string id)
    {
      bool result = false;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.DeleteEntry(id);
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

    public async Task<List<FileSystemEntry>> GetFolderChildsById(string id)
    {
      List<FileSystemEntry> result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.GetEntriesByParentId(id, EntryKind.Folder);
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

    public async Task<FileSystemEntry> GetFolderDetailById(string id)
    {
      FileSystemEntry result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.GetEntryById(id);
        if (result != null && result.EntryKind != EntryKind.Folder)
        {
          result = null;
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

    public async Task<List<FileSystemEntry>> GetFolderParentsById(string id)
    {
      List<FileSystemEntry> result = null;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.GetEntryParentsById(id);
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

    public async Task<FileSystemEntry> GetRootFolder()
    {
      FileSystemEntry result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.GetRootEntry();
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

    public async Task<FileSystemEntry> RenameFolder(string id, string name)
    {
      FileSystemEntry result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.RenameEntry(id, name);
        if (result != null && result.EntryKind != EntryKind.Folder)
        {
          result = null;
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

    public async Task<List<FileSystemEntry>> SearchFolder(string searchString)
    {
      List<FileSystemEntry> result = default;
      try
      {
        var repository = factory.Get(Archive.FileArchiveHierarchyType);

        result = await repository.SearchEntries(searchString, EntryKind.Folder);
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

    public Task<List<FileSystemEntry>> UploadFiles(List<FileSourceValueObject> filesToUpload, string folderId)
    {
      throw new NotImplementedException();
    }

    public Task<FileSystemEntry> GetFileById(string id)
    {
      throw new NotImplementedException();
    }

    public Task<FileSystemEntry> UploadNewVersion(string oldFileId, MemoryStream newFileData)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> GetFiles(List<string> ids)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> GetFolders(List<string> ids)
    {
      throw new NotImplementedException();
    }
  }
}
