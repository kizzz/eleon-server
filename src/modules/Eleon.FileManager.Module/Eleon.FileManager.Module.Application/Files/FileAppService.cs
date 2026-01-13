using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Folders;
using VPortal.FileManager.Module.VirtualFolders;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Files
{
  public class FileAppService : ModuleAppService, IFileAppService
  {
    private readonly FileDomainService fileDomainService;
    private readonly IVportalLogger<FileAppService> logger;

    public FileAppService(
        FileDomainService fileDomainService,
        IVportalLogger<FileAppService> logger)
    {
      this.fileDomainService = fileDomainService;
      this.logger = logger;
    }

    // Unified query methods
    public async Task<List<FileSystemEntryDto>> GetEntriesByParentId(string parentId, Guid archiveId, EntryKind? kind, List<FileStatus> fileStatuses, FileManagerType type, bool recursive)
    {
      List<FileSystemEntryDto> result = default;
      try
      {
        var entities = await fileDomainService.GetEntriesByParentId(parentId, archiveId, kind, fileStatuses, type, recursive);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entities);
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

    public async Task<PagedResultDto<FileSystemEntryDto>> GetEntriesByParentIdPaged(GetFileEntriesByParentPagedInput input, Guid archiveId, EntryKind? kind, List<FileStatus> fileStatuses, FileManagerType type, bool recursive)
    {
      PagedResultDto<FileSystemEntryDto> result = default;
      try
      {
        var pagedResult = await fileDomainService.GetEntriesByParentIdPaged(
            input.FolderId,
            archiveId,
            kind,
            fileStatuses,
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            type,
            recursive);

        var dtos = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(pagedResult.Items);
        result = new PagedResultDto<FileSystemEntryDto>(pagedResult.TotalCount, dtos);
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

    public async Task<List<FileSystemEntryDto>> GetEntriesByIds(List<string> ids, Guid archiveId, EntryKind? kind, FileManagerType type)
    {
      List<FileSystemEntryDto> result = default;
      try
      {
        var entities = await fileDomainService.GetEntriesByIds(ids, archiveId, kind, type);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entities);
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

    public async Task<FileSystemEntryDto> GetEntryById(string id, Guid archiveId, FileManagerType type)
    {
      FileSystemEntryDto result = default;
      try
      {
        var entity = await fileDomainService.GetEntryById(id, archiveId, type);
        result = ObjectMapper.Map<FileSystemEntry, FileSystemEntryDto>(entity);
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

    public async Task<List<HierarchyFolderDto>> GetEntryParentsById(string id, Guid archiveId, FileManagerType type)
    {
      List<HierarchyFolderDto> result = default;
      try
      {
        var entities = await fileDomainService.GetEntryParentsById(id, archiveId, type);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<HierarchyFolderDto>>(entities);
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

    public async Task<FileSystemEntryDto> GetRootEntry(Guid archiveId, FileManagerType type)
    {
      FileSystemEntryDto result = default;
      try
      {
        var entity = await fileDomainService.GetRootEntry(archiveId, type);
        result = ObjectMapper.Map<FileSystemEntry, FileSystemEntryDto>(entity);
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

    public async Task<List<FileSystemEntryDto>> SearchEntries(string search, Guid archiveId, EntryKind? kind, FileManagerType type)
    {
      List<FileSystemEntryDto> result = default;
      try
      {
        var entities = await fileDomainService.SearchEntries(search, archiveId, kind, type);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entities);
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
    public async Task<FileSystemEntryDto> CreateEntry(CreateEntryDto dto, Guid archiveId, FileManagerType type)
    {
      FileSystemEntryDto result = default;
      try
      {
        var entity = await fileDomainService.CreateEntry(dto, archiveId, type);
        result = ObjectMapper.Map<FileSystemEntry, FileSystemEntryDto>(entity);
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

    public async Task<FileSystemEntryDto> RenameEntry(RenameEntryDto dto, Guid archiveId, FileManagerType type)
    {
      FileSystemEntryDto result = default;
      try
      {
        var entity = await fileDomainService.RenameEntry(dto.Id, dto.Name, archiveId, type);
        result = ObjectMapper.Map<FileSystemEntry, FileSystemEntryDto>(entity);
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

    public async Task<bool> MoveEntry(MoveEntryDto dto, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.MoveEntry(dto.EntryId, dto.DestinationParentId, archiveId, type);
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

    public async Task<bool> MoveAllEntries(MoveAllEntriesDto dto, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.MoveAllEntries(dto.EntryIds, dto.DestinationParentId, archiveId, type);
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

    public async Task<bool> CopyEntry(CopyEntryDto dto, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.CopyEntry(dto.EntryId, dto.DestinationParentId, archiveId, type);
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

    public async Task<bool> DeleteEntry(string id, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.DeleteEntry(id, archiveId, type);
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

    // Old methods - to be removed
    public async Task<bool> CopyFile(CopyFileDto copyFileDto, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.CopyFile(
            copyFileDto.FileId,
            copyFileDto.DestinationFolderId,
            archiveId,
            type);
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

    public async Task<FileSystemEntryDto> CreateFolder(AddVirtualFolderDto dto, Guid archiveId, FileManagerType type)
    {
      FileSystemEntryDto result = default;
      try
      {
        var entity = await fileDomainService.CreateFolder(
            dto.Name,
            dto.PhysicalFolderId,
            dto.ParentId,
            archiveId,
            type);
        result = ObjectMapper.Map<FileSystemEntry, FileSystemEntryDto>(entity);
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

    public async Task<bool> DeleteFile(string id, Guid archiveId, FileManagerType type)
    {
      bool result = default;
      try
      {
        result = await fileDomainService.DeleteFile(id, archiveId, type);
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

    public async Task<bool> DeleteFileToken(Guid token, Guid archiveId, FileManagerType type)
    {
      bool result = default;
      try
      {
        result = await fileDomainService.DeleteFileToken(token, archiveId, type);
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

    public async Task<bool> DeleteFolder(string id, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.DeleteFolder(id, archiveId, type);
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

    public async Task<byte[]> DownloadFile(string id, bool isVersion, Guid archiveId, FileManagerType type)
    {
      byte[] result = default;
      try
      {
        result = await fileDomainService.DownloadFile(id, isVersion, archiveId, type);
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

    public async Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType type)
    {
      byte[] result = default;
      try
      {
        result = await fileDomainService.DownloadFileByToken(id, token, isVersion, archiveId, type);
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

    public async Task<FileSourceDto> FileViewer(string id, Guid archiveId, FileManagerType type)
    {
      FileSourceDto result = default;
      try
      {
        var entity = await fileDomainService.FileViewer(id, archiveId, type);
        result = ObjectMapper.Map<FileSourceValueObject, FileSourceDto>(entity);
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

    public async Task<List<FileSystemEntryDto>> GetFilesByFolderId(string id, Guid archiveId, List<FileStatus> fileStatuses, FileManagerType type)
    {
      List<FileSystemEntryDto> result = default;
      try
      {
        var entity = await fileDomainService.GetFilesByFolderId(id, archiveId, fileStatuses, type);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entity);
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

    public async Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion, Guid archiveId, FileManagerType type)
    {
      byte[] result = default;
      try
      {
        result = await fileDomainService.GetFileByToken(id, token, isVersion, archiveId, type);
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

    public async Task<string> GetFileToken(string id, bool isVersion, Guid archiveId, FileManagerType type)
    {
      string result = string.Empty;
      try
      {
        result = await fileDomainService.GetFileToken(id, isVersion, archiveId, type);
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

    public async Task<List<FileSystemEntryDto>> GetFolderChildsById(string id, Guid archiveId, List<FileStatus> fileStatuses, FileManagerType type)
    {
      List<FileSystemEntryDto> result = default;
      try
      {
        var entities = await fileDomainService.GetFolderChildsById(id, archiveId, fileStatuses, type);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entities);
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

    public async Task<FileSystemEntryDto> GetFolderDetailById(string id, Guid archiveId, FileManagerType type)
    {
      FileSystemEntryDto result = default;
      try
      {
        var entity = await fileDomainService.GetFolderDetailById(id, archiveId, type);
        result = ObjectMapper.Map<FileSystemEntry, FileSystemEntryDto>(entity);
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

    public async Task<List<HierarchyFolderDto>> GetFolderParentsById(string id, Guid archiveId, FileManagerType type)
    {
      List<HierarchyFolderDto> result = default;
      try
      {
        var entities = await fileDomainService.GetFolderParentsById(id, archiveId, type);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<HierarchyFolderDto>>(entities);
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

    public async Task<FileSystemEntryDto> GetRootFolder(Guid archiveId, FileManagerType type)
    {
      FileSystemEntryDto result = default;
      try
      {
        var entity = await fileDomainService.GetRootFolder(archiveId, type);
        result = ObjectMapper.Map<FileSystemEntry, FileSystemEntryDto>(entity);
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

    public async Task<bool> MoveAllFile(MoveAllFileDto moveAllFileDto, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.MoveAllFile(
            moveAllFileDto.FileIds,
            moveAllFileDto.Folders,
            moveAllFileDto.DestinationFolderId,
            archiveId,
            type);
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

    public async Task<bool> MoveFile(MoveFileDto moveFileDto, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.MoveFile(
            moveFileDto.FileId,
            moveFileDto.DestinationFolderId,
            archiveId,
            type
        );
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

    public async Task<List<string>> ReadTextFile(string id, bool isVersion, Guid archiveId, FileManagerType type)
    {
      List<string> result = default;
      try
      {
        result = await fileDomainService.ReadTextFile(id, isVersion, archiveId, type);
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

    public async Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType type)
    {
      List<string> result = default;
      try
      {
        result = await fileDomainService.ReadTextFileByToken(id, token, isVersion, archiveId, type);
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

    public async Task<bool> RenameFile(RenameFileDto renameFileDto, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.RenameFile(
            renameFileDto.Id,
            renameFileDto.Name,
            archiveId,
            type);
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

    public async Task<FileSystemEntryDto> RenameFolder(RenameVirtualFolderDto dto, Guid archiveId, FileManagerType type)
    {
      FileSystemEntryDto result = default;
      try
      {
        var entity = await fileDomainService.RenameFolder(dto.Id, dto.Name, archiveId, type);
        result = ObjectMapper.Map<FileSystemEntry, FileSystemEntryDto>(entity);
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

    public async Task<List<FileSystemEntryDto>> SearchFile(string searchString, Guid archiveId, FileManagerType type)
    {
      List<FileSystemEntryDto> result = default;
      try
      {
        var entities = await fileDomainService.SearchFile(searchString, archiveId, type);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entities);
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

    public async Task<List<FileSystemEntryDto>> SearchFolder(string searchString, Guid archiveId, FileManagerType type)
    {
      List<FileSystemEntryDto> result = default;
      try
      {
        var entities = await fileDomainService.SearchFolder(searchString, archiveId, type);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entities);
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

    public async Task<List<FileSystemEntryDto>> GetEntries(EntryFilterDto entryFilterDto)
    {
      List<FileSystemEntryDto> result = null;
      try
      {
        var entities = await fileDomainService.GetAllEntries(
            entryFilterDto.ArchiveId,
            entryFilterDto.Kind,
            entryFilterDto.FilterByFavourite,
            entryFilterDto.FilterByStatus,
            entryFilterDto.FilterByShareStatus,
            entryFilterDto.FileStatuses,
            entryFilterDto.FileShareStatuses,
            entryFilterDto.FileManagerType);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entities);
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

    public async Task<bool> RestoreEntry(string id, Guid archiveId, FileManagerType type)
    {
      bool result = false;
      try
      {
        result = await fileDomainService.RestoreEntry(id, archiveId, type);
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

    public async Task<List<FileSystemEntryDto>> GetEntryHistory(string id, Guid archiveId, FileManagerType type)
    {
      List<FileSystemEntryDto> result = new();
      try
      {
        var entities = await fileDomainService.GetFileHistory(id, archiveId, type);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entities);
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

    public async Task<List<FileSystemEntryDto>> UploadFiles(FileUploadDto fileUploadDto)
    {
      List<FileSystemEntryDto> result = default;
      try
      {
        var filesToUploadValueObjects = ObjectMapper.Map<List<FileSourceDto>, List<FileSourceValueObject>>(fileUploadDto.Files);
        var entities = await fileDomainService.UploadFiles(filesToUploadValueObjects, fileUploadDto.FolderId, fileUploadDto.ArchiveId, fileUploadDto.FileManagerType);
        result = ObjectMapper.Map<List<FileSystemEntry>, List<FileSystemEntryDto>>(entities);
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

    public async Task<byte[]> DownloadAll(DownloadAllDto downloadAllDto)
    {

      byte[] result = null;
      try
      {
        result = await fileDomainService.DownloadAllFiles(downloadAllDto.ArchiveId, downloadAllDto.Folders, downloadAllDto.FileIds, downloadAllDto.ParentId, downloadAllDto.FileManagerType);
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
