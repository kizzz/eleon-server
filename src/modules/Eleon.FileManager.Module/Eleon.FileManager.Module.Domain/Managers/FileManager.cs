using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Managers
{
  public class FileManager : DomainService
  {
    private readonly IVportalLogger<FileManager> logger;
    private readonly IFileFactory fileFactory;
    private readonly IArchiveRepository archiveRepository;

    public FileManager(
        IVportalLogger<FileManager> logger,
        IFileFactory fileFactory,
        IArchiveRepository archiveRepository)
    {
      this.logger = logger;
      this.fileFactory = fileFactory;
      this.archiveRepository = archiveRepository;
    }

    public async Task<FileSystemEntry> GetEntryById(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.GetEntryById(id);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<List<FileSystemEntry>> GetEntriesByParentId(string id, Guid archiveId, EntryKind? kind, FileManagerType fileManagerType, bool recursive)
    {
      List<FileSystemEntry> result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.GetEntriesByParentId(id, kind, recursive);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<(List<FileSystemEntry> Items, long TotalCount)> GetEntriesByParentIdPaged(string id, Guid archiveId, EntryKind? kind, int skipCount, int maxResultCount, string? sorting, FileManagerType fileManagerType, bool recursive)
    {
      (List<FileSystemEntry> Items, long TotalCount) result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.GetEntriesByParentIdPaged(id, kind, skipCount, maxResultCount, sorting, recursive);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<List<FileSystemEntry>> GetEntriesByIds(Guid archiveId, List<string> ids, EntryKind? kind, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.GetEntriesByIds(ids, kind);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<string> GetFileToken(string id, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      string result = string.Empty;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.GetFileToken(id, isVersion);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<bool> DeleteFileToken(Guid token, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.DeleteFileToken(token);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      byte[] result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.GetFileByToken(id, token, isVersion);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<bool> MoveEntry(string entryId, string destinationParentId, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.MoveEntry(entryId, destinationParentId);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<bool> MoveAllEntries(List<string> entryIds, string destinationParentId, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.MoveAllEntries(entryIds, destinationParentId);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<bool> CopyEntry(string entryId, string destinationParentId, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.CopyEntry(entryId, destinationParentId);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<FileSourceValueObject> FileViewer(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSourceValueObject result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.FileViewer(id);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<bool> DeleteEntry(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.DeleteEntry(id);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<byte[]> DownloadFile(string id, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      byte[] result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.DownloadFile(id, isVersion);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      byte[] result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.DownloadFileByToken(id, token, isVersion);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<List<string>> ReadTextFile(string id, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      List<string> result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.ReadTextFile(id, isVersion);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      List<string> result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.ReadTextFileByToken(id, token, isVersion);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<List<FileSystemEntry>> SearchEntries(string search, Guid archiveId, EntryKind? kind, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.SearchEntries(search, kind);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<FileSystemEntry> RenameEntry(string id, string name, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.RenameEntry(id, name);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<List<FileSystemEntry>> GetEntryParentsById(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        if (fileArchiveEntity != null)
        {
          var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
          result = await repository.GetEntryParentsById(id);
          result = result.Reverse<FileSystemEntry>().ToList();
        }
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<FileSystemEntry> GetRootEntry(Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.GetRootEntry();
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<List<FileSystemEntry>> UploadFiles(List<FileSourceValueObject> filesToUpload, string folderId, Guid archiveId, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.UploadFiles(filesToUpload, folderId);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<FileSystemEntry> UploadNewVersion(Guid archiveId, string oldFileId, MemoryStream newFileData, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, fileManagerType);
        var repository = await fileFactory.Get(fileArchiveEntity, fileManagerType);
        result = await repository.UploadNewVersion(oldFileId, newFileData);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }

    public async Task<List<FileSystemEntry>> GetFileHistory(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = new List<FileSystemEntry>();
      try
      {
        if (string.IsNullOrEmpty(id))
          return result;

        var file = await GetEntryById(id, archiveId, fileManagerType);
        result.Add(file);

        var parent = await GetFileHistory(file.ParentId, archiveId, fileManagerType);
        result.AddRange(parent);
      }
      catch (Exception e) { logger.Capture(e); }
      return result;
    }
    private async Task<FileArchiveEntity> GetFileArchiveEntityAsync(Guid archiveId, FileManagerType fileManagerType)
    {
      switch (fileManagerType)
      {
        case FileManagerType.FileArchive:
          {
            var archive = await archiveRepository.GetAsync(archiveId);
            if (!archive.IsActive)
            {
              throw new Exception("Trying to use inactive FileArchive");
            }
            return archive;
          }
        case FileManagerType.Provider:
          return new FileArchiveEntity(Guid.Empty)
          {
            StorageProviderId = archiveId,
            FileArchiveHierarchyType = FileArchiveHierarchyType.Physical,
            RootFolderId = "./"
          };
        default:
          return null;
      }
    }

    // Domain-level methods migrated from FileSystemEntryDomainService
    // These delegate to IFileSystemEntryRepository without validation/logic

    public async Task<FileSystemEntry> CreateFile(
        string id,
        Guid archiveId,
        string name,
        string? parentId,
        string? extension = null,
        string? path = null,
        string? size = null,
        string? thumbnailPath = null
      )
    {
      try
      {
        // Validate name
        if (string.IsNullOrWhiteSpace(name))
        {
          throw new UserFriendlyException("File name cannot be empty or whitespace.");
        }
        if (name != Path.GetFileName(name) ||
            name.Contains("..", StringComparison.Ordinal) ||
            name.Contains('<', StringComparison.Ordinal) ||
            name.Contains('>', StringComparison.Ordinal) ||
            name.Contains('\0') ||
            name.Contains("script", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("javascript:", StringComparison.OrdinalIgnoreCase))
        {
          throw new UserFriendlyException("File name contains invalid characters.");
        }

        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, FileManagerType.FileArchive);
        var repository = await fileFactory.Get(fileArchiveEntity, FileManagerType.FileArchive);

        // Validate parent exists and is a folder (if provided)
        if (!string.IsNullOrEmpty(parentId))
        {
          try
          {
            var parent = await repository.GetEntryById(parentId);
            if (parent.EntryKind != EntryKind.Folder)
            {
              throw new UserFriendlyException("Parent must be a folder.");
            }
          }
          catch (EntityNotFoundException)
          {
            throw new UserFriendlyException("Parent folder not found.");
          }
        }

        // Check for duplicate sibling names
        var siblings = await repository.GetEntriesByParentId(parentId, EntryKind.File);
        if (siblings.Any(s => s.Name == name && s.Id != id))
        {
          throw new UserFriendlyException($"A file with the name '{name}' already exists in this location.");
        }

        var result = await repository.CreateEntry(EntryKind.File, name, parentId, null, false, extension, path, size, thumbnailPath);
        return result;
      }
      catch (UserFriendlyException)
      {
        throw;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
    }

    public async Task<FileSystemEntry> CreateFolder(
        string id,
        Guid archiveId,
        string name,
        string? parentId,
        string? physicalFolderId = null,
        bool isShared = false,
        string? size = null)
    {
      try
      {
        // Validate name
        if (string.IsNullOrWhiteSpace(name))
        {
          throw new UserFriendlyException("Folder name cannot be empty or whitespace.");
        }

        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, FileManagerType.FileArchive);
        var repository = await fileFactory.Get(fileArchiveEntity, FileManagerType.FileArchive);

        // Validate parent exists and is a folder (if provided)
        if (!string.IsNullOrEmpty(parentId))
        {
          try
          {
            var parent = await repository.GetEntryById(parentId);
            if (parent.EntryKind != EntryKind.Folder)
            {
              throw new UserFriendlyException("Parent must be a folder.");
            }
          }
          catch (EntityNotFoundException)
          {
            throw new UserFriendlyException("Parent folder not found.");
          }
        }

        // Check for duplicate sibling names
        var siblings = await repository.GetEntriesByParentId(parentId, EntryKind.Folder);
        if (siblings.Any(s => s.Name == name && s.Id != id))
        {
          throw new UserFriendlyException($"A folder with the name '{name}' already exists in this location.");
        }

        var result = await repository.CreateEntry(EntryKind.Folder, name, parentId, physicalFolderId, isShared, null, null, size, null);
        return result;
      }
      catch (UserFriendlyException)
      {
        throw;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
    }

    public async Task<FileSystemEntry> Rename(string id, string newName, Guid archiveId)
    {
      try
      {
        // Validate name
        if (string.IsNullOrWhiteSpace(newName))
        {
          throw new UserFriendlyException("Name cannot be empty or whitespace.");
        }

        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, FileManagerType.FileArchive);
        var repository = await fileFactory.Get(fileArchiveEntity, FileManagerType.FileArchive);

        var entry = await repository.GetEntryById(id);

        // Check for duplicate sibling names
        var siblings = await repository.GetEntriesByParentId(entry.ParentId, entry.EntryKind);
        if (siblings.Any(s => s.Name == newName && s.Id != id))
        {
          throw new UserFriendlyException($"An entry with the name '{newName}' already exists in this location.");
        }

        var result = await repository.RenameEntry(id, newName);
        return result;
      }
      catch (UserFriendlyException)
      {
        throw;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
    }

    public async Task<FileSystemEntry> Move(string id, string destinationParentId, Guid archiveId)
    {
      try
      {
        // Cannot move entry under itself
        if (id == destinationParentId)
        {
          throw new UserFriendlyException("Cannot move an entry under itself.");
        }

        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, FileManagerType.FileArchive);
        var repository = await fileFactory.Get(fileArchiveEntity, FileManagerType.FileArchive);

        var entry = await repository.GetEntryById(id);

        // Validate destination exists and is a folder (if provided)
        if (!string.IsNullOrEmpty(destinationParentId))
        {
          try
          {
            var destination = await repository.GetEntryById(destinationParentId);
            if (destination.EntryKind != EntryKind.Folder)
            {
              throw new UserFriendlyException("Destination must be a folder.");
            }

            // Check if destination is a descendant (would create a cycle)
            var parents = await repository.GetEntryParentsById(destinationParentId);
            if (parents.Any(p => p.Id == id))
            {
              throw new UserFriendlyException("Cannot move an entry under one of its descendants.");
            }
          }
          catch (EntityNotFoundException)
          {
            throw new UserFriendlyException("Destination folder not found.");
          }
        }

        // Check for duplicate name at destination
        var siblingsAtDestination = await repository.GetEntriesByParentId(destinationParentId, entry.EntryKind);
        if (siblingsAtDestination.Any(s => s.Name == entry.Name && s.Id != id))
        {
          throw new UserFriendlyException($"An entry with the name '{entry.Name}' already exists in the destination.");
        }

        var moved = await repository.MoveEntry(id, destinationParentId);
        if (!moved)
        {
          throw new Exception("Failed to move entry.");
        }

        var result = await repository.GetEntryById(id);
        return result;
      }
      catch (UserFriendlyException)
      {
        throw;
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
    }

    public async Task Delete(string id, Guid archiveId)
    {
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, FileManagerType.FileArchive);
        var repository = await fileFactory.Get(fileArchiveEntity, FileManagerType.FileArchive);
        await repository.DeleteEntry(id);
      }
      catch (Exception e) { logger.Capture(e); }
    }

    public async Task<List<FileSystemEntry>> GetChildren(string parentId, Guid archiveId)
    {
      List<FileSystemEntry> result = default;
      try
      {
        var fileArchiveEntity = await GetFileArchiveEntityAsync(archiveId, FileManagerType.FileArchive);
        var repository = await fileFactory.Get(fileArchiveEntity, FileManagerType.FileArchive);
        result = await repository.GetEntriesByParentId(parentId);
      }
      catch (Exception e) { logger.Capture(e); }
      return result ?? new List<FileSystemEntry>();
    }
  }
}
