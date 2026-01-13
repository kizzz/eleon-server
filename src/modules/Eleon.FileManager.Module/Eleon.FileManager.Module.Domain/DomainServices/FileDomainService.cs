using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Factories;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.DomainServices
{
  public class FileDomainService : DomainService
  {
    private readonly IVportalLogger<FileDomainService> logger;
    private readonly IObjectMapper objectMapper;
    private readonly Managers.FileManager fileManager;
    private readonly ICurrentUser currentUser;
    private readonly FileArchivePermissionCheckerDomainService permissionChecker;
    private readonly IArchiveRepository archiveRepository;
    private readonly ICompressionFactory compressionFactory;
    private readonly FileStatusDomainService fileStatusDomainService;
    private readonly FileArchiveFavouriteDomainService favouriteDomainService;
    private readonly FileExternalLinkDomainService fileExternalLinkDomainService;
    private readonly IdentityUserManager userManager;

    public FileDomainService(
        IVportalLogger<FileDomainService> logger,
        IObjectMapper objectMapper,
        Managers.FileManager fileManager,
        ICurrentUser currentUser,
        FileArchivePermissionCheckerDomainService permissionChecker,
        ICompressionFactory compressionFactory,
        FileStatusDomainService fileStatusDomainService,
        FileArchiveFavouriteDomainService favouriteDomainService,
        FileExternalLinkDomainService fileExternalLinkDomainService,
        IdentityUserManager userManager,
        IArchiveRepository archiveRepository)
    {
      this.logger = logger;
      this.objectMapper = objectMapper;
      this.fileManager = fileManager;
      this.currentUser = currentUser;
      this.permissionChecker = permissionChecker;
      this.compressionFactory = compressionFactory;
      this.fileStatusDomainService = fileStatusDomainService;
      this.favouriteDomainService = favouriteDomainService;
      this.fileExternalLinkDomainService = fileExternalLinkDomainService;
      this.userManager = userManager;
      this.archiveRepository = archiveRepository;
    }

    public async Task<List<FileSystemEntry>> GetAllFiles(
     Guid archiveId,
     bool filterByFavourite,
     bool filterByStatus,
     bool filterByShareStatus,
     List<FileStatus> fileStatuses,
     List<FileShareStatus> fileShareStatuses,
     FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        List<string> fileIds = new();
        List<string> favouritedIds = new();

        favouritedIds = (await favouriteDomainService.GetListAsync(archiveId, null))
            .Select(t => t.FileId)
            .Where(t => !t.IsNullOrEmpty())
            .ToList();

        List<FileExternalLinkEntity> fileExternalLinkEntities =
            await fileExternalLinkDomainService.GetLinksAsync(archiveId, fileShareStatuses);

        var fileStatusEntities =
            await fileStatusDomainService.GetFileListAsync(archiveId, fileStatuses);

        if (filterByFavourite)
          fileIds = favouritedIds;
        else if (filterByShareStatus && !fileShareStatuses.IsNullOrEmpty())
          fileIds = fileExternalLinkEntities.Select(t => t.FileId).ToList();
        else if (filterByStatus && !fileStatuses.IsNullOrEmpty())
          fileIds = fileStatusEntities.Select(t => t.FileId).ToList();

        result = await fileManager.GetEntriesByIds(archiveId, fileIds, EntryKind.File, fileManagerType);
        result = await permissionChecker.GetPermited(archiveId, result, FileManagerPermissionType.Read, fileManagerType);

        foreach (var file in result.Where(f => f.EntryKind == EntryKind.File))
        {
          var link = fileExternalLinkEntities.FirstOrDefault(t => t.FileId == file.Id);
          if (link != null)
            file.SharedStatus = link.PermissionType;

          var status = fileStatusEntities.FirstOrDefault(t => t.FileId == file.Id);
          if (status != null)
            file.Status = status.FileStatus;

          file.IsFavourite = favouritedIds.Contains(file.Id);
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

    public async Task<List<FileSystemEntry>> GetAllFolders(
Guid archiveId,
bool filterByFavourite,
bool filterByStatus,
List<FileStatus> fileStatuses,
FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        List<string> folderIds = new();
        List<string> favouritedIds = new();

        favouritedIds = (await favouriteDomainService.GetListAsync(archiveId, null))
            .Select(t => t.FolderId)
            .Where(t => !t.IsNullOrEmpty())
            .ToList();

        var fileStatusEntities = await fileStatusDomainService.GetFolderListAsync(archiveId, fileStatuses);

        if (filterByFavourite)
          folderIds = favouritedIds;
        else if (filterByStatus && !fileStatuses.IsNullOrEmpty())
          folderIds = fileStatusEntities.Select(t => t.FileId).ToList();

        result = await fileManager.GetEntriesByIds(archiveId, folderIds, EntryKind.Folder, fileManagerType);
        result = await permissionChecker.GetPermited(archiveId, result, FileManagerPermissionType.Read, fileManagerType);

        foreach (var folder in result.Where(f => f.EntryKind == EntryKind.Folder))
        {
          var status = fileStatusEntities.FirstOrDefault(t => t.FileId == folder.Id);
          if (status != null)
            folder.Status = status.FileStatus;

          folder.IsFavourite = favouritedIds.Contains(folder.Id);
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

    // Unified query methods
    public async Task<List<FileSystemEntry>> GetEntriesByParentId(string parentId, Guid archiveId, EntryKind? kind, List<FileStatus> fileStatuses, FileManagerType fileManagerType, bool recursive)
    {
      List<FileSystemEntry> result = default;
      try
      {
        if (fileStatuses.IsNullOrEmpty())
          fileStatuses = new List<FileStatus> { FileStatus.Active };

        if (!(await permissionChecker.CheckPermission(archiveId, parentId, FileManagerPermissionType.Read, fileManagerType)))
          return new List<FileSystemEntry>();

        result = await fileManager.GetEntriesByParentId(parentId, archiveId, kind, fileManagerType, recursive);

        var archive = await archiveRepository.FindAsync(archiveId);

        if (archive != null && archive.FileArchiveHierarchyType == FileArchiveHierarchyType.Virtual)
        {
          var favorited = await favouriteDomainService.GetListAsync(archiveId, parentId);
          foreach (var entry in result)
          {
            if (entry.EntryKind == EntryKind.File)
              entry.IsFavourite = favorited.Any(f => f.FileId == entry.Id);
            else
              entry.IsFavourite = favorited.Any(f => f.FolderId == entry.Id);
          }

          foreach (var entry in result.Where(f => f.EntryKind == EntryKind.File))
          {
            var externalLink = await fileExternalLinkDomainService.GetAsync(entry.Id, entry.ArchiveId, true);
            entry.SharedStatus = externalLink.PermissionType;
          }

          await fileStatusDomainService.FillStatuses(archiveId, result.Where(f => f.EntryKind == EntryKind.File).ToList(), result.Where(f => f.EntryKind == EntryKind.Folder).ToList());
          result = result.Where(f => fileStatuses.Contains(f.Status)).ToList();
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

    public async Task<(List<FileSystemEntry> Items, long TotalCount)> GetEntriesByParentIdPaged(string parentId, Guid archiveId, EntryKind? kind, List<FileStatus> fileStatuses, int skipCount, int maxResultCount, string? sorting, FileManagerType fileManagerType, bool recursive)
    {
      (List<FileSystemEntry> Items, long TotalCount) result = default;
      try
      {
        if (fileStatuses.IsNullOrEmpty())
          fileStatuses = new List<FileStatus> { FileStatus.Active };

        if (!(await permissionChecker.CheckPermission(archiveId, parentId, FileManagerPermissionType.Read, fileManagerType)))
          return (new List<FileSystemEntry>(), 0);

        var pagedResult = await fileManager.GetEntriesByParentIdPaged(parentId, archiveId, kind, skipCount, maxResultCount, sorting, fileManagerType, recursive);

        var archive = await archiveRepository.FindAsync(archiveId);

        if (archive != null && archive.FileArchiveHierarchyType == FileArchiveHierarchyType.Virtual)
        {
          var favorited = await favouriteDomainService.GetListAsync(archiveId, parentId);
          foreach (var entry in pagedResult.Items)
          {
            if (entry.EntryKind == EntryKind.File)
              entry.IsFavourite = favorited.Any(f => f.FileId == entry.Id);
            else
              entry.IsFavourite = favorited.Any(f => f.FolderId == entry.Id);
          }

          foreach (var entry in pagedResult.Items.Where(f => f.EntryKind == EntryKind.File))
          {
            var externalLink = await fileExternalLinkDomainService.GetAsync(entry.Id, entry.ArchiveId, true);
            entry.SharedStatus = externalLink.PermissionType;
          }

          await fileStatusDomainService.FillStatuses(archiveId, pagedResult.Items.Where(f => f.EntryKind == EntryKind.File).ToList(), pagedResult.Items.Where(f => f.EntryKind == EntryKind.Folder).ToList());
          var filteredItems = pagedResult.Items.Where(f => fileStatuses.Contains(f.Status)).ToList();

          // Note: TotalCount may not be accurate after filtering by status, but we preserve it for simplicity
          // A more accurate implementation would require separate status filtering at the repository level
          result = (filteredItems, pagedResult.TotalCount);
        }
        else
        {
          result = pagedResult;
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

    public async Task<List<FileSystemEntry>> GetEntriesByIds(List<string> ids, Guid archiveId, EntryKind? kind, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        result = await fileManager.GetEntriesByIds(archiveId, ids, kind, fileManagerType);
        result = await permissionChecker.GetPermited(archiveId, result, FileManagerPermissionType.Read, fileManagerType);
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

    public async Task<FileSystemEntry> GetEntryById(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        result = await fileManager.GetEntryById(id, archiveId, fileManagerType);
        if (result != null && !(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
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

    public async Task<List<FileSystemEntry>> GetEntryParentsById(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        result = await fileManager.GetEntryParentsById(id, archiveId, fileManagerType);
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

    public async Task<FileSystemEntry> GetRootEntry(Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        result = await fileManager.GetRootEntry(archiveId, fileManagerType);
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

    public async Task<List<FileSystemEntry>> SearchEntries(string search, Guid archiveId, EntryKind? kind, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        result = await fileManager.SearchEntries(search, archiveId, kind, fileManagerType);
        result = await permissionChecker.GetPermited(archiveId, result, FileManagerPermissionType.Read, fileManagerType);
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
    public async Task<FileSystemEntry> CreateEntry(CreateEntryDto dto, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, dto.ParentId, FileManagerPermissionType.Write, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }

        var entryId = Guid.NewGuid().ToString();
        if (dto.Kind == EntryKind.File)
        {
          result = await fileManager.CreateFile(entryId, archiveId, dto.Name, dto.ParentId, dto.Extension, null, null, null);
        }
        else
        {
          result = await fileManager.CreateFolder(entryId, archiveId, dto.Name, dto.ParentId, dto.PhysicalFolderId, dto.IsShared, null);
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

    public async Task<FileSystemEntry> RenameEntry(string id, string name, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, id, FileManagerPermissionType.Modify, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }

        result = await fileManager.Rename(id, name, archiveId);
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

    public async Task<bool> MoveEntry(string entryId, string destinationParentId, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, entryId, FileManagerPermissionType.Modify, fileManagerType)))
          throw new AbpAuthorizationException();

        if (!(await permissionChecker.CheckPermission(archiveId, destinationParentId, FileManagerPermissionType.Modify, fileManagerType)))
          return result;

        await fileManager.Move(entryId, destinationParentId, archiveId);
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

    public async Task<bool> MoveAllEntries(List<string> entryIds, string destinationParentId, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        foreach (var entryId in entryIds)
        {
          if (!(await permissionChecker.CheckFilePermission(archiveId, entryId, FileManagerPermissionType.Modify, fileManagerType)))
            throw new AbpAuthorizationException();
        }

        if (!(await permissionChecker.CheckPermission(archiveId, destinationParentId, FileManagerPermissionType.Modify, fileManagerType)))
          throw new AbpAuthorizationException();

        foreach (var entryId in entryIds)
        {
          await fileManager.Move(entryId, destinationParentId, archiveId);
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

    public async Task<bool> CopyEntry(string entryId, string destinationParentId, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, entryId, FileManagerPermissionType.Modify, fileManagerType)))
          throw new AbpAuthorizationException();

        if (!(await permissionChecker.CheckPermission(archiveId, destinationParentId, FileManagerPermissionType.Modify, fileManagerType)))
          throw new AbpAuthorizationException();

        result = await fileManager.CopyEntry(entryId, destinationParentId, archiveId, fileManagerType);
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

    public async Task<bool> DeleteEntry(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        var entry = await fileManager.GetEntryById(id, archiveId, fileManagerType);
        if (entry == null)
          return false;

        if (!(await permissionChecker.CheckFilePermission(archiveId, id, FileManagerPermissionType.Modify, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }

        if (entry.EntryKind == EntryKind.File)
        {
          await fileStatusDomainService.UpdateFileStatus(archiveId, id, FileStatus.Trash);
          result = await favouriteDomainService.RemoveFromFavourites(archiveId, id, null);
        }
        else
        {
          await fileStatusDomainService.UpdateFolderStatus(archiveId, id, FileStatus.Trash);
          result = await favouriteDomainService.RemoveFromFavourites(archiveId, null, id);
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

    public async Task<List<FileSystemEntry>> GetFilesByFolderId(
string id,
Guid archiveId,
List<FileStatus> fileStatuses,
FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        if (fileStatuses.IsNullOrEmpty())
          fileStatuses = new List<FileStatus> { FileStatus.Active };

        if (!(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
          return new List<FileSystemEntry>();

        result = await fileManager.GetEntriesByParentId(id, archiveId, EntryKind.File, fileManagerType, false);

        var archive = await archiveRepository.FindAsync(archiveId);

        if (archive != null && archive.FileArchiveHierarchyType == FileArchiveHierarchyType.Virtual)
        {
          var favorited = await favouriteDomainService.GetListAsync(archiveId, id);
          foreach (var fileEntity in result)
            fileEntity.IsFavourite = favorited.Any(f => f.FileId == fileEntity.Id);

          foreach (var fileEntity in result)
          {
            var externalLink = await fileExternalLinkDomainService.GetAsync(fileEntity.Id, fileEntity.ArchiveId, true);
            fileEntity.SharedStatus = externalLink.PermissionType;
          }

          await fileStatusDomainService.FillStatuses(archiveId, result.Where(f => f.EntryKind == EntryKind.File).ToList(), new List<FileSystemEntry>());
          result = result.Where(f => f.EntryKind == EntryKind.File && fileStatuses.Contains(f.Status)).ToList();
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

    public async Task<string> GetFileToken(string id, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      string result = string.Empty;
      try
      {
        result = await fileManager.GetFileToken(id, isVersion, archiveId, fileManagerType);
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

    public async Task<bool> DeleteFileToken(Guid token, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        result = await fileManager.DeleteFileToken(token, archiveId, fileManagerType);
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

    public async Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      byte[] result = default;
      try
      {
        result = await fileManager.GetFileByToken(id, token, isVersion, archiveId, fileManagerType);
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

    public async Task<bool> MoveFile(string fileId, string destinationFolderId, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, fileId, FileManagerPermissionType.Modify, fileManagerType)))
          throw new AbpAuthorizationException();

        if (!(await permissionChecker.CheckPermission(archiveId, destinationFolderId, FileManagerPermissionType.Modify, fileManagerType)))
          return result;

        result = await fileManager.MoveEntry(fileId, destinationFolderId, archiveId, fileManagerType);
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

    public async Task<bool> MoveAllFile(List<string> fileIds, List<string> folders, string destinationFolderId, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        foreach (var folder in folders)
        {
          if (!(await permissionChecker.CheckPermission(archiveId, folder, FileManagerPermissionType.Modify, fileManagerType)))
            throw new AbpAuthorizationException();
        }

        foreach (var fileId in fileIds)
        {
          if (!(await permissionChecker.CheckFilePermission(archiveId, fileId, FileManagerPermissionType.Modify, fileManagerType)))
            throw new AbpAuthorizationException();
        }

        if (!(await permissionChecker.CheckPermission(archiveId, destinationFolderId, FileManagerPermissionType.Modify, fileManagerType)))
          throw new AbpAuthorizationException();

        var allEntryIds = new List<string>(fileIds);
        allEntryIds.AddRange(folders);
        result = await fileManager.MoveAllEntries(allEntryIds, destinationFolderId, archiveId, fileManagerType);
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

    public async Task<bool> CopyFile(string fileId, string destinationFolderId, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, fileId, FileManagerPermissionType.Modify, fileManagerType)))
          throw new AbpAuthorizationException();

        if (!(await permissionChecker.CheckPermission(archiveId, destinationFolderId, FileManagerPermissionType.Modify, fileManagerType)))
          throw new AbpAuthorizationException();

        result = await fileManager.CopyEntry(fileId, destinationFolderId, archiveId, fileManagerType);
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

    public async Task<FileSourceValueObject> FileViewer(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSourceValueObject result = default;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
          throw new AbpAuthorizationException();

        result = await fileManager.FileViewer(id, archiveId, fileManagerType);
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


    public async Task<bool> DeleteFile(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, id, FileManagerPermissionType.Modify, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }

        await fileStatusDomainService.UpdateFileStatus(archiveId, id, FileStatus.Trash);
        result = await favouriteDomainService.RemoveFromFavourites(archiveId, id, null);
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

    public async Task<List<FileSystemEntry>> GetAllEntries(
      Guid archiveId,
      EntryKind? kind,
      bool filterByFavourite,
      bool filterByStatus,
      bool filterByShareStatus,
      List<FileStatus> fileStatuses,
      List<FileShareStatus> fileShareStatuses,
      FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        if (kind == EntryKind.File)
        {
          result = await GetAllFiles(archiveId, filterByFavourite, filterByStatus, filterByShareStatus, fileStatuses, fileShareStatuses, fileManagerType);
        }
        else if (kind == EntryKind.Folder)
        {
          result = await GetAllFolders(archiveId, filterByFavourite, filterByStatus, fileStatuses, fileManagerType);
        }
        else
        {
          // Get both and combine
          var files = await GetAllFiles(archiveId, filterByFavourite, filterByStatus, filterByShareStatus, fileStatuses, fileShareStatuses, fileManagerType);
          var folders = await GetAllFolders(archiveId, filterByFavourite, filterByStatus, fileStatuses, fileManagerType);
          result = files.Concat(folders).ToList();
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

    public async Task<bool> RestoreEntry(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        var entry = await GetEntryById(id, archiveId, fileManagerType);
        if (entry == null)
          return false;

        if (entry.EntryKind == EntryKind.File)
        {
          if (!(await permissionChecker.CheckFilePermission(archiveId, id, FileManagerPermissionType.Modify, fileManagerType)))
          {
            throw new AbpAuthorizationException();
          }

          var fileStatus = await fileStatusDomainService.GetFileStatusAsync(archiveId, id);
          if (fileStatus == FileStatus.Trash)
          {
            result = await fileStatusDomainService.UpdateFileStatus(archiveId, id, FileStatus.Active);
          }
        }
        else // EntryKind.Folder
        {
          if (!(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Modify, fileManagerType)))
          {
            throw new AbpAuthorizationException();
          }

          var folderStatus = await fileStatusDomainService.GetFolderStatusAsync(archiveId, id);
          if (folderStatus == FileStatus.Trash)
          {
            result = await fileStatusDomainService.UpdateFolderStatus(archiveId, id, FileStatus.Active);
          }
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


    public async Task<byte[]> DownloadAllFiles(Guid archiveId, List<string> folderIds, List<string> fileIds, string parentFolderId, FileManagerType fileManagerType)
    {
      byte[] result = default;
      try
      {
        List<FileSystemEntry> filteredFolders = new();
        foreach (var folderId in folderIds)
        {
          var folder = await GetFolderDetailById(folderId, archiveId, fileManagerType);
          if (folder != null)
          {
            FileSystemEntry folderWithHierarchy = await GetFolderFullHierarchy(folder, archiveId, fileManagerType);
            filteredFolders.Add(folderWithHierarchy);
          }
        }

        List<FileSystemEntry> files = await GetFilesByFolderId(parentFolderId, archiveId, null, fileManagerType);
        var filteredFiles = files.Where(file => fileIds.Contains(file.Id)).ToList();

        foreach (var file in files)
        {
          file.Source = await DownloadFile(file.Id, false, archiveId, fileManagerType);
        }

        var folderValueObjects = objectMapper.Map<List<FileSystemEntry>, List<HierarchyFolderValueObject>>(filteredFolders);
        var fileValueObjects = objectMapper.Map<List<FileSystemEntry>, List<FileValueObject>>(filteredFiles);

        var compressionRepository = compressionFactory.Get();
        result = await compressionRepository.Compress(folderValueObjects, fileValueObjects);
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

    private async Task<FileSystemEntry> GetFolderFullHierarchy(FileSystemEntry folder, Guid archiveId, FileManagerType fileManagerType)
    {
      var children = await GetFolderChildsById(folder.Id, archiveId, null, fileManagerType);
      var files = await GetFilesByFolderId(folder.Id, archiveId, null, fileManagerType);

      foreach (var child in children)
      {
        await GetFolderFullHierarchy(child, archiveId, fileManagerType);
      }

      foreach (var file in files)
      {
        file.Source = await DownloadFile(file.Id, false, archiveId, fileManagerType);
      }

      folder.Children = children;
      folder.Files = files;
      return folder;
    }

    public async Task<byte[]> DownloadFile(string id, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      byte[] result = default;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }
        result = await fileManager.DownloadFile(id, isVersion, archiveId, fileManagerType);
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

    public async Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      byte[] result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }
        result = await fileManager.DownloadFileByToken(id, token, isVersion, archiveId, fileManagerType);
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

    public async Task<List<string>> ReadTextFile(string id, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      List<string> result = default;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }
        result = await fileManager.ReadTextFile(id, isVersion, archiveId, fileManagerType);
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

    public async Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType fileManagerType)
    {
      List<string> result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }
        result = await fileManager.ReadTextFileByToken(id, token, isVersion, archiveId, fileManagerType);
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

    public async Task<List<FileSystemEntry>> SearchFile(string searchString, Guid archiveId, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        // TODO: Artem, search permissions
        result = await fileManager.SearchEntries(searchString, archiveId, EntryKind.File, fileManagerType);
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

    public async Task<bool> RenameFile(string id, string name, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, id, FileManagerPermissionType.Modify, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }
        var entry = await fileManager.RenameEntry(id, name, archiveId, fileManagerType);
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

    public async Task<FileSystemEntry> CreateFolder(string name, string folderId, string parentId, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, parentId, FileManagerPermissionType.Write, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }
        var entryId = Guid.NewGuid().ToString();
        result = await fileManager.CreateFolder(entryId, archiveId, name, parentId, folderId, false, null);
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

    public async Task<bool> DeleteFolder(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      bool result = false;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Modify, fileManagerType)))
        {
          throw new AbpAuthorizationException();
        }
        await fileStatusDomainService.UpdateFolderStatus(archiveId, id, FileStatus.Trash);
        result = await favouriteDomainService.RemoveFromFavourites(archiveId, null, id);
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


    public async Task<List<FileSystemEntry>> GetFolderChildsById(string id, Guid archiveId, List<FileStatus> fileStatuses, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        if (fileStatuses.IsNullOrEmpty())
        {
          fileStatuses = new List<FileStatus> { FileStatus.Active };
        }

        if (!(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
        {
          return result;
        }

        result = await fileManager.GetEntriesByParentId(id, archiveId, EntryKind.Folder, fileManagerType, false);

        var archive = await archiveRepository.FindAsync(archiveId);

        if (archive != null && archive.FileArchiveHierarchyType == FileArchiveHierarchyType.Virtual)
        {

          var favorited = await favouriteDomainService.GetListAsync(archiveId, id);
          foreach (var folderEntity in result)
          {
            if (favorited.Any(f => f.FolderId == folderEntity.Id))
            {
              folderEntity.IsFavourite = true;
            }
          }

          await fileStatusDomainService.FillStatuses(archiveId, new List<FileSystemEntry>(), result);
          result = result.Where(f => fileStatuses.Contains(f.Status)).ToList();
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

    public async Task<FileSystemEntry> GetFolderDetailById(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
        {
          return result;
        }
        result = await fileManager.GetEntryById(id, archiveId, fileManagerType);
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

    public async Task<List<FileSystemEntry>> GetFolderParentsById(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
        {
          return result;
        }
        result = await fileManager.GetEntryParentsById(id, archiveId, fileManagerType);
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

    public async Task<FileSystemEntry> GetRootFolder(Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, null, FileManagerPermissionType.Read, fileManagerType)))
        {
          return result;
        }
        result = await fileManager.GetRootEntry(archiveId, fileManagerType);
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

    public async Task<FileSystemEntry> RenameFolder(string id, string name, Guid archiveId, FileManagerType fileManagerType)
    {
      FileSystemEntry result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, id, FileManagerPermissionType.Modify, fileManagerType)))
        {
          return result;
        }
        result = await fileManager.RenameEntry(id, name, archiveId, fileManagerType);
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

    public async Task<List<FileSystemEntry>> SearchFolder(string searchString, Guid archiveId, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, null, FileManagerPermissionType.Read, fileManagerType)))
        {
          return result;
        }
        result = await fileManager.SearchEntries(searchString, archiveId, EntryKind.Folder, fileManagerType);
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

    public async Task<List<FileSystemEntry>> UploadFiles(List<FileSourceValueObject> filesToUpload, string folderId, Guid archiveId, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        if (!(await permissionChecker.CheckPermission(archiveId, folderId, FileManagerPermissionType.Write, fileManagerType)))
        {
          return result;
        }

        result = await fileManager.UploadFiles(filesToUpload, folderId, archiveId, fileManagerType);
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

    public async Task<List<FileSystemEntry>> GetFileHistory(string id, Guid archiveId, FileManagerType fileManagerType)
    {
      List<FileSystemEntry> result = default;
      try
      {
        if (!(await permissionChecker.CheckFilePermission(archiveId, id, FileManagerPermissionType.Read, fileManagerType)))
        {
          return result;
        }

        result = await fileManager.GetFileHistory(id, archiveId, fileManagerType);
        if (result != null && result.Count > 0)
        {
          foreach (var file in result)
          {
            var userId = !file.LastModifierId.HasValue ? file.CreatorId.Value : file.LastModifierId.Value;
            var lastModifier = await userManager.GetByIdAsync(userId);
            if (lastModifier != null)
            {
              file.LastModifierName = $"{lastModifier.Name} {lastModifier.Surname}";
            }
          }
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


  }
}
