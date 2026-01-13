using Org.BouncyCastle.Utilities;
using SharedModule.modules.Blob.Module.Shared;
using SharedModule.modules.Blob.Module.VfsShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Helpers;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Repositories.File
{
  public class FileStorageProviderRepository : IFileSystemEntryRepository
  {
    private readonly IVfsBlobProvider _blobProvider;
    private readonly ICurrentUser _currentUser;

    public FileArchiveEntity Archive { get; set; }

    public FileStorageProviderRepository(IVfsBlobProvider blobProvider, ICurrentUser currentUser)
    {
      _blobProvider = blobProvider ?? throw new ArgumentNullException(nameof(blobProvider));
      _currentUser = currentUser;
    }

    // Unified query methods
    public async Task<List<FileSystemEntry>> GetEntriesByParentId(string parentId, EntryKind? kind = null, bool recursive = false)
    {
      if (kind == EntryKind.File)
        return await GetFilesByFolderId(parentId);
      else if (kind == EntryKind.Folder)
        return await GetFolderChildsById(parentId);
      else
      {
        var files = await GetFilesByFolderId(parentId);
        var folders = await GetFolderChildsById(parentId);
        return files.Concat(folders).ToList();
      }
    }

    public async Task<(List<FileSystemEntry> Items, long TotalCount)> GetEntriesByParentIdPaged(string parentId, EntryKind? kind, int skipCount, int maxResultCount, string? sorting = null, bool recursive = false)
    {
      var folderKey = FilePathHelper.NormalizePath(parentId);
      var pagedArgs = FilePathHelper.CreateListPagedArgs(Archive, _currentUser.Id.ToString(), folderKey, skipCount, maxResultCount, false);
      var pagedResult = await _blobProvider.ListPagedAsync(pagedArgs);

      var allEntries = new List<FileSystemEntry>();

      // Filter by EntryKind and convert to FileSystemEntry
      foreach (var item in pagedResult.Items)
      {
        if (kind == EntryKind.File && item.IsFolder)
          continue;
        if (kind == EntryKind.Folder && !item.IsFolder)
          continue;

        FileSystemEntry entry = null;

        entry = new FileSystemEntry(FilePathHelper.CombinePaths(folderKey, item.Key));
        entry.ArchiveId = Archive.Id;
        entry.Name = FilePathHelper.GetFileNameFromKey(item.Key);
        entry.ParentId = parentId;

        if (item.IsFolder)
        {
          entry.EntryKind = EntryKind.Folder;
          entry.IsShared = false;
        }
        else
        {
          entry.EntryKind = EntryKind.File;
          entry.Size = item.Size.ToString();
        }

        entry.LastModificationTime = item.LastModified;
        allEntries.Add(entry);
      }

      // Apply sorting if provided (simple implementation - can be enhanced)
      if (!string.IsNullOrEmpty(sorting))
      {
        var sortParts = sorting.Split(' ');
        var sortField = sortParts[0];
        var sortDirection = sortParts.Length > 1 && sortParts[1].Equals("desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

        allEntries = sortField.ToLowerInvariant() switch
        {
          "name" => sortDirection == "desc"
            ? allEntries.OrderByDescending(e => e.Name).ToList()
            : allEntries.OrderBy(e => e.Name).ToList(),
          "lastmodificationtime" => sortDirection == "desc"
            ? allEntries.OrderByDescending(e => e.LastModificationTime).ToList()
            : allEntries.OrderBy(e => e.LastModificationTime).ToList(),
          "size" => sortDirection == "desc"
            ? allEntries.OrderByDescending(e => long.TryParse(e.Size, out var s) ? s : 0).ToList()
            : allEntries.OrderBy(e => long.TryParse(e.Size, out var s) ? s : 0).ToList(),
          _ => allEntries
        };
      }

      // Note: TotalCount from provider represents all items (files + folders) in the folder
      // When filtering by EntryKind, the actual total may differ, but we return the provider's count
      // for simplicity. A more accurate implementation would require separate counts per kind.
      return (allEntries, pagedResult.TotalCount);
    }

    public async Task<List<FileSystemEntry>> GetEntriesByIds(List<string> ids, EntryKind? kind = null)
    {
      if (kind == EntryKind.File)
        return await GetFiles(ids);
      else if (kind == EntryKind.Folder)
        return await GetFolders(ids);
      else
      {
        var files = await GetFiles(ids);
        var folders = await GetFolders(ids);
        return files.Concat(folders).ToList();
      }
    }

    public async Task<FileSystemEntry> GetEntryById(string id)
    {
      var file = await GetFileById(id);
      if (file != null)
        return file;
      return await GetFolderDetailById(id);
    }

    public async Task<List<FileSystemEntry>> GetEntryParentsById(string id)
    {
      return await GetFolderParentsById(id);
    }

    public async Task<FileSystemEntry> GetRootEntry()
    {
      return await GetRootFolder();
    }

    public async Task<List<FileSystemEntry>> SearchEntries(string search, EntryKind? kind = null)
    {
      if (kind == EntryKind.File)
        return await SearchFile(search);
      else if (kind == EntryKind.Folder)
        return await SearchFolder(search);
      else
      {
        var files = await SearchFile(search);
        var folders = await SearchFolder(search);
        return files.Concat(folders).ToList();
      }
    }

    // Unified mutation methods
    public async Task<FileSystemEntry> CreateEntry(EntryKind kind, string name, string? parentId, string? physicalFolderId = null, bool isShared = false, string? extension = null, string? path = null, string? size = null, string? thumbnailPath = null)
    {
      if (kind == EntryKind.File)
        throw new NotImplementedException("File creation not implemented in FileStorageProviderRepository");
      else
      {
        var folderId = physicalFolderId ?? Guid.NewGuid().ToString();
        return await CreateFolder(name, folderId, parentId);
      }
    }

    public async Task<FileSystemEntry> RenameEntry(string id, string name)
    {
      var entry = await GetEntryById(id);
      if (entry == null)
        throw new Exception("Entry not found");

      if (entry.EntryKind == EntryKind.File)
      {
        await RenameFile(id, name);
        return await GetFileById(id);
      }
      else
      {
        return await RenameFolder(id, name);
      }
    }

    private async Task<FileSystemEntry> RenameFileWithReturn(string id, string name)
    {
      await RenameFile(id, name);
      return await GetFileById(id);
    }

    public async Task<bool> MoveEntry(string entryId, string destinationParentId)
    {
      var entry = await GetEntryById(entryId);
      if (entry == null)
        return false;

      if (entry.EntryKind == EntryKind.File)
        return await MoveFile(entryId, destinationParentId);
      else
        return await MoveFolder(entryId, destinationParentId);
    }

    public async Task<bool> MoveAllEntries(List<string> entryIds, string destinationParentId)
    {
      bool success = true;
      foreach (var id in entryIds)
      {
        success &= await MoveEntry(id, destinationParentId);
      }
      return success;
    }

    public async Task<bool> CopyEntry(string entryId, string destinationParentId)
    {
      var entry = await GetEntryById(entryId);
      if (entry == null)
        return false;

      if (entry.EntryKind == EntryKind.File)
        return await CopyFile(entryId, destinationParentId);
      else
        throw new NotImplementedException("Folder copy not implemented");
    }

    public async Task<bool> DeleteEntry(string id)
    {
      var entry = await GetEntryById(id);
      if (entry == null)
        return false;

      if (entry.EntryKind == EntryKind.File)
        return await DeleteFile(id);
      else
        return await DeleteFolder(id);
    }

    // FILES --------------------------------------------------

    private async Task<List<FileSystemEntry>> GetFilesByFolderId(string id)
    {
      var folderKey = FilePathHelper.NormalizePath(id);
      var items = await _blobProvider.ListAsync(FilePathHelper.CreateListArgs(Archive, _currentUser.Id.ToString(), folderKey));

      return items
        .Where(x => !x.IsFolder)
        .Select(x => new FileSystemEntry(FilePathHelper.CombinePaths(folderKey, x.Key))
        {
          ArchiveId = Archive.Id,
          Name = FilePathHelper.GetFileNameFromKey(x.Key),
          ParentId = id,
          EntryKind = EntryKind.File,
          Size = x.Size.ToString()
        })
        .Select(e => { e.LastModificationTime = items.First(i => FilePathHelper.CombinePaths(folderKey, i.Key) == e.Id).LastModified; return e; })
        .ToList();
    }

    private async Task<List<FileSystemEntry>> GetFiles(List<string> ids)
    {
      var result = new List<FileSystemEntry>();

      foreach (var id in ids)
      {
        var key = FilePathHelper.NormalizePath(id);
        var f = await GetFileById(key);
        if (f != null)
          result.Add(f);
      }

      return result;
    }

    public Task<string> GetFileToken(string id, bool isVersion)
    {
      var key = FilePathHelper.NormalizePath(id);
      return Task.FromResult(key);
    }

    public Task<bool> DeleteFileToken(Guid token)
        => Task.FromResult(true);

    public Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion)
    {
      var key = FilePathHelper.NormalizePath(id);
      return DownloadFile(key, isVersion);
    }

    private async Task<bool> MoveFile(string fileId, string destinationFolderId)
    {
      var sourceKey = FilePathHelper.NormalizePath(fileId);
      var destFolderKey = FilePathHelper.NormalizePath(destinationFolderId);
      var destKey = FilePathHelper.CombinePaths(destFolderKey, FilePathHelper.GetFileNameFromKey(sourceKey));

      var bytes = await _blobProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, _currentUser.Id.ToString(), sourceKey));
      if (bytes == null)
        return false;

      using var ms = FilePathHelper.ToStream(bytes);
      await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), destKey, ms));
      await _blobProvider.DeleteAsync(FilePathHelper.CreateDeleteArgs(Archive, _currentUser.Id.ToString(), sourceKey));

      return true;
    }

    private async Task<bool> MoveAllFile(List<string> fileIds, List<string> folders, string destinationFolderId)
    {
      bool success = true;

      foreach (var id in fileIds)
        success &= await MoveFile(id, destinationFolderId);

      foreach (var f in folders)
        success &= await MoveFolder(f, destinationFolderId);

      return success;
    }

    public async Task<bool> MoveFolder(string folderId, string destinationDir)
    {
      var folderKey = FilePathHelper.NormalizePath(folderId);
      var destBase = FilePathHelper.CombinePaths(FilePathHelper.NormalizePath(destinationDir), FilePathHelper.GetFileNameFromKey(folderId));

      var items = await _blobProvider.ListAsync(FilePathHelper.CreateListArgs(Archive, _currentUser.Id.ToString(), folderKey));

      foreach (var item in items)
      {
        var source = FilePathHelper.CombinePaths(folderKey, item.Key);
        var dest = FilePathHelper.CombinePaths(destBase, item.Key);

        if (item.IsFolder)
        {
          using var ms = new MemoryStream(Array.Empty<byte>());
          await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), dest, ms, true));
        }
        else
        {
          var bytes = await _blobProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, _currentUser.Id.ToString(), source));
          if (bytes != null)
          {
            using var ms = FilePathHelper.ToStream(bytes);
            await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), dest, ms));
          }
        }

        await _blobProvider.DeleteAsync(FilePathHelper.CreateDeleteArgs(Archive, _currentUser.Id.ToString(), source));
      }

      await _blobProvider.DeleteAsync(FilePathHelper.CreateDeleteArgs(Archive, _currentUser.Id.ToString(), folderKey));
      return true;
    }

    private async Task<bool> CopyFile(string fileId, string destinationFolderId)
    {
      var sourceKey = FilePathHelper.NormalizePath(fileId);
      var destFolderKey = FilePathHelper.NormalizePath(destinationFolderId);
      var destKey = FilePathHelper.CombinePaths(destFolderKey, FilePathHelper.GetFileNameFromKey(sourceKey));

      var bytes = await _blobProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, _currentUser.Id.ToString(), sourceKey));
      if (bytes == null)
        return false;

      using var ms = FilePathHelper.ToStream(bytes);
      await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), destKey, ms));

      return true;
    }

    private async Task<bool> DeleteFile(string id)
    {
      var key = FilePathHelper.NormalizePath(id);
      return await _blobProvider.DeleteAsync(FilePathHelper.CreateDeleteArgs(Archive, _currentUser.Id.ToString(), key));
    }

    public async Task<FileSourceValueObject> FileViewer(string id)
    {
      var key = FilePathHelper.NormalizePath(id);
      var bytes = await _blobProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, _currentUser.Id.ToString(), key)) ?? Array.Empty<byte>();

      return new FileSourceValueObject
      {
        Name = FilePathHelper.GetFileNameFromKey(key),
        Source = Convert.ToBase64String(bytes)
      };
    }

    public async Task<byte[]> DownloadFile(string id, bool isVersion)
    {
      var key = FilePathHelper.NormalizePath(id);
      return await _blobProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, _currentUser.Id.ToString(), key)) ?? Array.Empty<byte>();
    }

    public Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion)
    {
      var key = FilePathHelper.NormalizePath(id);
      return DownloadFile(key, isVersion);
    }

    public async Task<List<string>> ReadTextFile(string id, bool isVersion)
    {
      var key = FilePathHelper.NormalizePath(id);
      var bytes = await DownloadFile(key, isVersion);
      var text = Encoding.UTF8.GetString(bytes);

      return text
        .Replace("\r\n", "\n")
        .Split('\n')
        .ToList();
    }

    public Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion)
    {
      var key = FilePathHelper.NormalizePath(id);
      return ReadTextFile(key, isVersion);
    }

    private async Task<List<FileSystemEntry>> SearchFile(string searchString)
    {
      var items = await _blobProvider.ListAsync(FilePathHelper.CreateListArgs(Archive, _currentUser.Id.ToString(), string.Empty));

      return items
        .Where(x => !x.IsFolder &&
                    FilePathHelper.GetFileNameFromKey(x.Key).Contains(searchString, StringComparison.OrdinalIgnoreCase))
        .Select(x =>
        new FileSystemEntry(x.Key)
        {
          ArchiveId = Archive.Id,
          Name = FilePathHelper.GetFileNameFromKey(x.Key),
          EntryKind = EntryKind.File,
          Size = x.Size.ToString()
        })
        .ToList();
    }

    private async Task<bool> RenameFile(string id, string newName)
    {
      var oldKey = FilePathHelper.NormalizePath(id);
      var parent = FilePathHelper.GetParentKey(oldKey);
      var newKey = FilePathHelper.CombinePaths(parent, newName);

      var bytes = await _blobProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, _currentUser.Id.ToString(), oldKey));
      if (bytes == null)
        return false;

      using var ms = FilePathHelper.ToStream(bytes);
      await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), newKey, ms));
      await _blobProvider.DeleteAsync(FilePathHelper.CreateDeleteArgs(Archive, _currentUser.Id.ToString(), oldKey));

      return true;
    }

    public async Task<List<FileSystemEntry>> UploadFiles(List<FileSourceValueObject> files, string folderId = null)
    {
      var result = new List<FileSystemEntry>();
      if (string.IsNullOrEmpty(folderId))
        folderId = Archive.RootFolderId;
      var baseFolder = FilePathHelper.NormalizePath(folderId);

      foreach (var f in files)
      {
        var fileName = f.Name ?? Guid.NewGuid().ToString("N");
        var key = FilePathHelper.CombinePaths(baseFolder, fileName);

        using var stream = FilePathHelper.ToStream(f.Source);
        await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), key, stream));

        result.Add(new FileSystemEntry(key)
        {
          ArchiveId = Archive.Id,
          Name = fileName,
          ParentId = folderId,
          EntryKind = EntryKind.File,
          Size = stream.Length.ToString()
        }); // ThumbnailPath
      }

      return result;
    }

    public async Task<FileSystemEntry> UploadNewVersion(string oldFileId, MemoryStream newFileData)
    {
      var key = FilePathHelper.NormalizePath(oldFileId);
      newFileData.Position = 0;

      await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), key, newFileData));

      return new FileSystemEntry(key)
      {
        ArchiveId = Archive.Id,
        Name = FilePathHelper.GetFileNameFromKey(key),
        ParentId = null, // ParentId
        Extension = null, // Extension
        Size = newFileData.Length.ToString(), // Size
        EntryKind = EntryKind.File
      };
    }

    private async Task<FileSystemEntry> GetFileById(string id)
    {
      var key = FilePathHelper.NormalizePath(id);
      var bytes = await _blobProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, _currentUser.Id.ToString(), key));
      if (bytes == null)
        return null;

      return new FileSystemEntry(key)
      {
        ArchiveId = Archive.Id,
        Name = FilePathHelper.GetFileNameFromKey(key),
        EntryKind = EntryKind.File,
        Size = bytes.LongLength.ToString()
      };
    }

    // FOLDERS --------------------------------------------------

    private async Task<List<FileSystemEntry>> GetFolders(List<string> ids)
    {
      var result = new List<FileSystemEntry>();

      foreach (var id in ids)
      {
        var key = FilePathHelper.NormalizePath(id);
        var folder = await GetFolderDetailById(key);
        if (folder != null)
          result.Add(folder);
      }

      return result;
    }

    private async Task<FileSystemEntry> CreateFolder(string name, string folderId, string parentId)
    {
      var parentKey = FilePathHelper.NormalizePath(parentId);
      var folderKey = FilePathHelper.CombinePaths(parentKey, name);

      using var ms = new MemoryStream(Array.Empty<byte>());
      await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), folderKey, ms, true));

      return new FileSystemEntry(folderKey)
      {
        ArchiveId = Archive.Id,
        Name = name,
        ParentId = parentId,
        EntryKind = EntryKind.Folder
      };
    }

    private async Task<bool> DeleteFolder(string id)
    {
      var folderKey = FilePathHelper.NormalizePath(id);
      var items = await _blobProvider.ListAsync(FilePathHelper.CreateListArgs(Archive, _currentUser.Id.ToString(), folderKey));

      foreach (var item in items)
      {
        var childKey = FilePathHelper.CombinePaths(folderKey, item.Key);
        await _blobProvider.DeleteAsync(FilePathHelper.CreateDeleteArgs(Archive, _currentUser.Id.ToString(), childKey));
      }

      await _blobProvider.DeleteAsync(FilePathHelper.CreateDeleteArgs(Archive, _currentUser.Id.ToString(), folderKey));
      return true;
    }

    private async Task<List<FileSystemEntry>> GetFolderChildsById(string id)
    {
      var folderKey = FilePathHelper.NormalizePath(id);
      var items = await _blobProvider.ListAsync(FilePathHelper.CreateListArgs(Archive, _currentUser.Id.ToString(), folderKey));

      return items
        .Where(x => x.IsFolder && !x.Key.TrimEnd('/').Contains('/'))
        .Select(x =>
        new FileSystemEntry(FilePathHelper.CombinePaths(folderKey, x.Key))
        {
          ArchiveId = Archive.Id,
          Name = FilePathHelper.GetFileNameFromKey(x.Key),
          ParentId = id,
          EntryKind = EntryKind.Folder,
          LastModificationTime = x.LastModified

        })
        .ToList();
    }

    private Task<FileSystemEntry> GetFolderDetailById(string id)
    {
      var key = FilePathHelper.NormalizePath(id);

      return Task.FromResult(new FileSystemEntry(key)
      {
        ArchiveId = Archive.Id,
        Name = string.IsNullOrEmpty(key) ? "Root" : FilePathHelper.GetFileNameFromKey(key),
        EntryKind = EntryKind.Folder
      });
    }

    private Task<List<FileSystemEntry>> GetFolderParentsById(string id)
    {
      var result = new List<FileSystemEntry>();
      var key = $"{FilePathHelper.NormalizePath(id)}/";

      while (!string.IsNullOrEmpty(key) && key != "/")
      {
        key = FilePathHelper.GetParentKey(key);
        if (!string.IsNullOrEmpty(key))
        {
          result.Add(new FileSystemEntry(key)
          {
            ArchiveId = Archive.Id,
            Name = FilePathHelper.GetFileNameFromKey(key),
            EntryKind = EntryKind.Folder
          });
        }
      }

      return Task.FromResult(result);
    }

    private Task<FileSystemEntry> GetRootFolder()
    {
      return Task.FromResult(new FileSystemEntry(string.Empty)
      {
        ArchiveId = Archive.Id,
        Name = "Root",
        EntryKind = EntryKind.Folder
      });
    }

    private async Task<FileSystemEntry> RenameFolder(string id, string name)
    {
      var oldKey = FilePathHelper.NormalizePath(id);
      var parent = FilePathHelper.GetParentKey(oldKey);
      var newKey = FilePathHelper.CombinePaths(parent, name);

      var items = await _blobProvider.ListAsync(FilePathHelper.CreateListArgs(Archive, _currentUser.Id.ToString(), oldKey));

      foreach (var item in items)
      {
        var source = FilePathHelper.CombinePaths(oldKey, item.Key);
        var dest = FilePathHelper.CombinePaths(newKey, item.Key);

        if (item.IsFolder)
        {
          using var ms = new MemoryStream(Array.Empty<byte>());
          await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), dest, ms, true));
        }
        else
        {
          var bytes = await _blobProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, _currentUser.Id.ToString(), source));
          if (bytes != null)
          {
            using var ms = FilePathHelper.ToStream(bytes);
            await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), dest, ms));
          }
        }

        await _blobProvider.DeleteAsync(FilePathHelper.CreateDeleteArgs(Archive, _currentUser.Id.ToString(), source));
      }

      await _blobProvider.DeleteAsync(FilePathHelper.CreateDeleteArgs(Archive, _currentUser.Id.ToString(), oldKey));

      using var markerStream = new MemoryStream();
      await _blobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, _currentUser.Id.ToString(), newKey, markerStream));

      return new FileSystemEntry(newKey)
      {
        ArchiveId = Archive.Id,
        Name = name,
        ParentId = parent,
        EntryKind = EntryKind.Folder
      };
    }

    private async Task<List<FileSystemEntry>> SearchFolder(string searchString)
    {
      var items = await _blobProvider.ListAsync(FilePathHelper.CreateListArgs(Archive, _currentUser.Id.ToString(), string.Empty));

      return items
        .Where(x => x.IsFolder &&
                    x.Key.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        .Select(x => new FileSystemEntry(x.Key)
        {
          ArchiveId = Archive.Id,
          Name = FilePathHelper.GetFileNameFromKey(x.Key),
          EntryKind = EntryKind.Folder
        })
        .ToList();
    }
  }
}
