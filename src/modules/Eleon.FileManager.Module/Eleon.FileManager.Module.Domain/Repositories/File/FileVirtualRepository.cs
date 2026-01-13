using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Utilities;
using SharedModule.modules.Blob.Module.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Helpers;
using VPortal.FileManager.Module.ValueObjects;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.DynamicOptions;

namespace VPortal.FileManager.Module.Repositories.File
{
  public class FileVirtualRepository : IFileSystemEntryRepository, ITransientDependency
  {
    private readonly IVportalLogger<FileVirtualRepository> logger;
    private readonly IBasicRepository<FileSystemEntry, string> entryRepository;
    private readonly ICurrentUser currentUser;
    private readonly IGuidGenerator guidGenerator;
    private readonly IStringLocalizer<VPortal.FileManager.Module.Localization.ModuleResource> localizer;
    private readonly VfsStorageProviderCacheManager vfsStorageProviderCacheService;

    public FileArchiveEntity Archive { get; set; }

    public FileVirtualRepository(
        IVportalLogger<FileVirtualRepository> logger,
        IBasicRepository<FileSystemEntry, string> entryRepository,
        IGuidGenerator guidGenerator,
        IPhysicalFolderRepository physicalFolderRepository,
        ICurrentUser currentUser,
        IStringLocalizer<Localization.ModuleResource> localizer,
        VfsStorageProviderCacheManager vfsStorageProviderCacheService)
    {
      this.logger = logger;
      this.entryRepository = entryRepository;
      this.guidGenerator = guidGenerator;
      this.localizer = localizer;
      this.currentUser = currentUser;
      this.vfsStorageProviderCacheService = vfsStorageProviderCacheService;
    }

    // Unified query methods
    public async Task<List<FileSystemEntry>> GetEntriesByParentId(
  string parentId,
  EntryKind? kind = null,
  bool recursive = false)
    {
      var result = new List<FileSystemEntry>();

      try
      {
        var dbSet = await entryRepository.GetDbSetAsync();

        // Always get direct children first (and/or all descendants if recursive)
        // Use AsNoTracking for read-only speed + less memory (remove if you plan to mutate and save).
        var queryRoot = dbSet.AsNoTracking().Where(e => e.ParentId == parentId);

        if (!recursive)
        {
          if (kind.HasValue)
            queryRoot = queryRoot.Where(e => e.EntryKind == kind.Value);

          return await queryRoot.ToListAsync();
        }

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var frontierParentIds = new List<string> { parentId };

        visited.Add(parentId);

        while (frontierParentIds.Count > 0)
        {
          var children = await dbSet.AsNoTracking()
            .Where(e => frontierParentIds.Contains(e.ParentId))
            .ToListAsync();

          if (children.Count == 0)
            break;

          result.AddRange(children);

          frontierParentIds = children
            .Where(c => c.EntryKind == EntryKind.Folder)
            .Select(c => c.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id) && visited.Add(id))
            .ToList();
        }

        if (kind.HasValue)
          result = result.Where(e => e.EntryKind == kind.Value).ToList();
      }
      catch (Exception e)
      {
        logger.Capture(e);
        // keep returning empty list rather than null (safer for callers)
        result = new List<FileSystemEntry>();
      }
      finally
      {
      }

      return result;
    }


    public async Task<(List<FileSystemEntry> Items, long TotalCount)> GetEntriesByParentIdPaged(
   string parentId,
   EntryKind? kind,
   int skipCount,
   int maxResultCount,
   string? sorting = null,
   bool recursive = false)
    {
      var items = new List<FileSystemEntry>();
      long totalCount = 0;

      try
      {
        var dbSet = await entryRepository.GetDbSetAsync();

        // Non-recursive: keep your DB-level paging/sorting (including size special-case)
        if (!recursive)
        {
          var query = dbSet.AsNoTracking().Where(f => f.ParentId == parentId);

          if (kind.HasValue)
            query = query.Where(f => f.EntryKind == kind.Value);

          totalCount = await query.CountAsync();

          var sortParts = !string.IsNullOrWhiteSpace(sorting) ? sorting.Split(' ', StringSplitOptions.RemoveEmptyEntries) : new[] { "name" };
          var sortField = sortParts[0].ToLowerInvariant();
          var sortDesc = sortParts.Length > 1 && sortParts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
          var isSizeSort = sortField == "size";

          if (isSizeSort)
          {
            var allItems = await query.ToListAsync();
            items = (sortDesc
                ? allItems.OrderByDescending(e => e.Size != null && long.TryParse(e.Size, out var s) ? s : 0L)
                : allItems.OrderBy(e => e.Size != null && long.TryParse(e.Size, out var s) ? s : 0L))
              .Skip(skipCount)
              .Take(maxResultCount)
              .ToList();
          }
          else
          {
            query = sortField switch
            {
              "name" => sortDesc ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
              "lastmodificationtime" => sortDesc ? query.OrderByDescending(e => e.LastModificationTime) : query.OrderBy(e => e.LastModificationTime),
              _ => query.OrderBy(e => e.Name)
            };

            items = await query
              .Skip(skipCount)
              .Take(maxResultCount)
              .ToListAsync();
          }

          return (items, totalCount);
        }

        var all = new List<FileSystemEntry>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { parentId };
        var frontierParentIds = new List<string> { parentId };

        while (frontierParentIds.Count > 0)
        {
          var children = await dbSet.AsNoTracking()
            .Where(e => frontierParentIds.Contains(e.ParentId))
            .ToListAsync();

          if (children.Count == 0)
            break;

          all.AddRange(children);

          // Next frontier: folders only (avoid querying children of files)
          frontierParentIds = children
            .Where(c => c.EntryKind == EntryKind.Folder)
            .Select(c => c.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id) && visited.Add(id))
            .ToList();
        }

        IEnumerable<FileSystemEntry> filtered = all;
        if (kind.HasValue)
          filtered = filtered.Where(e => e.EntryKind == kind.Value);

        // Total after filtering (recursive total)
        var filteredList = filtered.ToList();
        totalCount = filteredList.Count;

        // Sorting (in-memory)
        var sortPartsRec = !string.IsNullOrWhiteSpace(sorting) ? sorting.Split(' ', StringSplitOptions.RemoveEmptyEntries) : new[] { "name" };
        var sortFieldRec = sortPartsRec[0].ToLowerInvariant();
        var sortDescRec = sortPartsRec.Length > 1 && sortPartsRec[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        Func<FileSystemEntry, long> sizeKey = e =>
          e.Size != null && long.TryParse(e.Size, out var s) ? s : 0L;

        IOrderedEnumerable<FileSystemEntry> ordered = sortFieldRec switch
        {
          "size" => sortDescRec ? filteredList.OrderByDescending(sizeKey) : filteredList.OrderBy(sizeKey),
          "lastmodificationtime" => sortDescRec
            ? filteredList.OrderByDescending(e => e.LastModificationTime)
            : filteredList.OrderBy(e => e.LastModificationTime),
          "name" or _ => sortDescRec
            ? filteredList.OrderByDescending(e => e.Name)
            : filteredList.OrderBy(e => e.Name)
        };

        items = ordered
          .Skip(skipCount)
          .Take(maxResultCount)
          .ToList();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return (items, totalCount);
    }


    public async Task<List<FileSystemEntry>> GetEntriesByIds(List<string> ids, EntryKind? kind = null)
    {
      List<FileSystemEntry> result = null;
      try
      {
        var dbSet = await entryRepository.GetDbSetAsync();
        var query = dbSet.Where(f => ids.Contains(f.Id));

        if (kind.HasValue)
        {
          query = query.Where(f => f.EntryKind == kind.Value);
        }

        result = await query.ToListAsync();
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

    public async Task<FileSystemEntry> GetEntryById(string id)
    {
      FileSystemEntry result = null;
      try
      {
        result = await entryRepository.GetAsync(id);
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

    public async Task<List<FileSystemEntry>> GetEntryParentsById(string id)
    {
      List<FileSystemEntry> result = new List<FileSystemEntry>();
      try
      {
        var entry = await entryRepository.GetAsync(id, true);
        if (entry != null)
        {
          result.Add(entry);
          if (string.IsNullOrEmpty(entry.ParentId))
          {
            return result;
          }
          result.AddRange(await GetEntryParentsById(entry.ParentId));
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

    public async Task<FileSystemEntry> GetRootEntry()
    {
      FileSystemEntry result = null;
      try
      {
        var dbSet = await entryRepository.GetDbSetAsync();
        result = await dbSet
            .Where(f => f.ParentId == null && f.EntryKind == EntryKind.Folder)
            .FirstOrDefaultAsync();
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

    public async Task<List<FileSystemEntry>> SearchEntries(string search, EntryKind? kind = null)
    {
      List<FileSystemEntry> result = null;
      try
      {
        var dbSet = await entryRepository.GetDbSetAsync();
        var likePattern = string.IsNullOrWhiteSpace(search) || search.Contains('%') || search.Contains('_')
            ? search
            : $"%{search}%";
        var query = dbSet.Where(f => EF.Functions.Like(f.Name, likePattern));

        if (kind.HasValue)
        {
          query = query.Where(f => f.EntryKind == kind.Value);
        }

        result = await query.ToListAsync();
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

    // Unified mutation methods
    public async Task<FileSystemEntry> CreateEntry(
        EntryKind kind,
        string name,
        string? parentId,
        string? physicalFolderId = null,
        bool isShared = false,
        string? extension = null,
        string? path = null,
        string? size = null,
        string? thumbnailPath = null
      )
    {
      FileSystemEntry result = null;
      try
      {
        result = new FileSystemEntry(Guid.NewGuid().ToString());
        result.ArchiveId = Archive.Id;
        result.Name = name;
        result.ParentId = parentId;
        result.Size = size;
        if (kind == EntryKind.File)
        {
          result.EntryKind = EntryKind.File;
          result.Extension = extension;
          result.Path = path;
          result.ThumbnailPath = thumbnailPath;
        }
        else
        {
          result.EntryKind = EntryKind.Folder;
          result.PhysicalFolderId = physicalFolderId;
          result.IsShared = isShared;
        }

        result.LastModificationTime = DateTime.Now;
        result = await entryRepository.InsertAsync(result, true);
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

    public async Task<FileSystemEntry> RenameEntry(string id, string name)
    {
      FileSystemEntry result = null;
      try
      {
        result = await entryRepository.GetAsync(id, true);
        if (result == null)
        {
          throw new Exception("Entry not found");
        }

        var siblings = await GetEntriesByParentId(result.ParentId, result.EntryKind);
        if (siblings.Any(f => f.Name == name && f.Id != id))
        {
          var entryType = result.EntryKind == EntryKind.File ? "File" : "Folder";
          throw new UserFriendlyException($"{entryType}NameAlreadyExists");
        }

        result.Name = name;
        await entryRepository.UpdateAsync(result, true);
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

    public async Task<bool> MoveEntry(string entryId, string destinationParentId)
    {
      bool result = false;
      try
      {
        var entry = await entryRepository.FindAsync(entryId);
        if (entry == null)
        {
          throw new Exception("Entry not found");
        }

        entry.ParentId = destinationParentId;
        if (entry.EntryKind == EntryKind.File)
        {
          entry.FolderId = destinationParentId; // Legacy mapping
        }

        await entryRepository.UpdateAsync(entry, true);
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

    public async Task<bool> MoveAllEntries(List<string> entryIds, string destinationParentId)
    {
      bool result = false;
      try
      {
        foreach (var entryId in entryIds)
        {
          await MoveEntry(entryId, destinationParentId);
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

    public async Task<bool> CopyEntry(string entryId, string destinationParentId)
    {
      bool result = false;
      try
      {
        var sourceEntry = await entryRepository.FindAsync(entryId);
        if (sourceEntry == null)
        {
          throw new Exception("Entry not found");
        }

        FileSystemEntry newEntry = new FileSystemEntry(guidGenerator.Create().ToString()); // Changed from newEntry() to new FileSystemEntry()
        newEntry.ArchiveId = Archive.Id;
        newEntry.Name = sourceEntry.Name;
        newEntry.ParentId = destinationParentId;

        newEntry.Size = sourceEntry.Size;

        if (sourceEntry.EntryKind == EntryKind.File)
        {
          newEntry.EntryKind = EntryKind.File;
          newEntry.Extension = sourceEntry.Extension;
          newEntry.Path = sourceEntry.Path;
          newEntry.ThumbnailPath = sourceEntry.ThumbnailPath;
        }
        else
        {
          newEntry.EntryKind = EntryKind.Folder;
          newEntry.IsShared = sourceEntry.IsShared;
          newEntry.Size = sourceEntry.Size;
        }

        await entryRepository.InsertAsync(newEntry, true);
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

    public async Task<bool> DeleteEntry(string id)
    {
      bool result = false;
      try
      {
        var entry = await entryRepository.GetAsync(id);
        if (entry != null)
        {
          await entryRepository.UpdateAsync(entry, true);
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

    public async Task<byte[]> DownloadFile(string id, bool isVersion)
    {

      byte[] result = null;
      try
      {
        var vfsProvider = await ResolveVfsBlobProvider();
        var fileBytes = await vfsProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, currentUser.Id.ToString(), id));

        if (fileBytes == null)
        {
          throw new Exception("FileNotFound");
        }

        result = fileBytes;
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

      string result = null;
      try
      {
        var vfsProvider = await ResolveVfsBlobProvider();
        var bytes = await vfsProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, currentUser.Id.ToString(), id));

        if (bytes == null)
        {
          throw new Exception("FileNotFound");
        }

        result = Convert.ToBase64String(bytes);
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

    public Task<bool> DeleteFileToken(Guid token)
    {
      throw new NotImplementedException();
    }

    public async Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion)
    {
      byte[] result = null;
      try
      {
        var vfsProvider = await ResolveVfsBlobProvider();
        var fileBytes = await vfsProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, currentUser.Id.ToString(), id));

        if (fileBytes == null)
        {
          throw new Exception("FileNotFound");
        }

        result = fileBytes;
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
      FileSourceValueObject result = new FileSourceValueObject();
      try
      {
        var vfsProvider = await ResolveVfsBlobProvider();
        var fileBytes = await vfsProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, currentUser.Id.ToString(), id));

        if (fileBytes == null)
        {
          throw new Exception("FileNotFound");
        }

        result.Source = Convert.ToBase64String(fileBytes);
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
      byte[] result = null;
      try
      {
        var vfsProvider = await ResolveVfsBlobProvider();
        var fileBytes = await vfsProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, currentUser.Id.ToString(), id));

        if (fileBytes == null)
        {
          throw new Exception("FileNotFound");
        }

        result = fileBytes;
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
      List<string> result = null;
      try
      {
        var vfsProvider = await ResolveVfsBlobProvider();
        var fileBytes = await vfsProvider.GetAllBytesOrNullAsync(FilePathHelper.CreateGetArgs(Archive, currentUser.Id.ToString(), id));

        if (fileBytes == null)
        {
          throw new Exception("FileNotFound");
        }

        result = new List<string>();
        result.Add(Convert.ToBase64String(fileBytes));
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

    public Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion)
    {
      throw new NotImplementedException();
    }

    public async Task<List<FileSystemEntry>> UploadFiles(List<FileSourceValueObject> filesToUpload, string folderId = null)
    {

      List<FileSystemEntry> result = new List<FileSystemEntry>();
      try
      {
        var vfsProvider = await ResolveVfsBlobProvider();

        if (string.IsNullOrEmpty(folderId))
        {
          folderId = Archive.RootFolderId;
        }

        foreach (var file in filesToUpload)
        {
          var fileFolderId = folderId;

          if (file.RelativePath != null)
          {
            var paths = file.RelativePath.Split("/").SkipLast(1);

            foreach (var path in paths)
            {
              var children = await GetEntriesByParentId(fileFolderId, EntryKind.Folder);
              var folder = children.FirstOrDefault(f => f.Name == path);
              if (folder == null)
              {
                folder = await CreateEntry(
                  EntryKind.Folder,
                  path,
                  fileFolderId);
              }

              fileFolderId = folder.Id;
            }
          }

          var folderFiles = await GetEntriesByParentId(fileFolderId, EntryKind.File);

          if (folderFiles.Any(f => f.Name == file.Name))
          {
            throw new UserFriendlyException(localizer["FileNameAlreadyExists"]);
          }

          var fileEntity = new FileSystemEntry(guidGenerator.Create().ToString());
          fileEntity.ArchiveId = Archive.Id;
          fileEntity.FolderId = fileFolderId;
          fileEntity.ParentId = fileFolderId;
          fileEntity.Extension = file.Type;
          fileEntity.EntryKind = EntryKind.File;
          fileEntity.Name = file.Name;

          fileEntity.LastModificationTime = DateTime.Now;

          byte[] data = Convert.FromBase64String(file.Source);
          fileEntity.Size = data.LongLength.ToString();

          using var ms = FilePathHelper.ToStream(data);
          await vfsProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, currentUser.Id.ToString(), fileEntity.Id, ms));

          var uploadedFile = await entryRepository.InsertAsync(fileEntity, true);
          result.Add(uploadedFile);
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

    public async Task<FileSystemEntry> UploadNewVersion(string oldFileId, MemoryStream newFileData)
    {

      FileSystemEntry result = null;
      try
      {
        var oldEntity = await entryRepository
            .GetAsync(oldFileId);

        if (oldEntity == null || oldEntity.EntryKind != EntryKind.File)
        {
          throw new Exception("File not found");
        }

        result = new FileSystemEntry(oldEntity, guidGenerator.Create().ToString());

        oldEntity.Size = newFileData.Length.ToString();
        //oldEntity.Status = FileStatus.Archived;

        await entryRepository.UpdateAsync(oldEntity, true);

        var vfsBlobProvider = await ResolveVfsBlobProvider();
        await vfsBlobProvider.SaveAsync(FilePathHelper.CreateSaveArgs(Archive, currentUser.Id.ToString(), result.Id, newFileData));

        await entryRepository.InsertAsync(result, true);
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

    private async Task<IVfsBlobProvider> ResolveVfsBlobProvider()
    {
      IVfsBlobProvider result = null;
      try
      {
        result = await vfsStorageProviderCacheService.ResolveProviderAsync(Archive.TenantId, Archive.StorageProviderId.ToString());
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
