using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Files;
using VPortal.FileManager.Module.Folders;
using VPortal.FileManager.Module.VirtualFolders;

namespace VPortal.FileManager.Module.Files
{
  public interface IFileAppService : IApplicationService
  {
    // Unified query methods
    Task<List<FileSystemEntryDto>> GetEntriesByParentId(string parentId, Guid archiveId, EntryKind? kind, List<FileStatus> fileStatuses, FileManagerType type, bool recursive);
    Task<PagedResultDto<FileSystemEntryDto>> GetEntriesByParentIdPaged(GetFileEntriesByParentPagedInput input, Guid archiveId, EntryKind? kind, List<FileStatus> fileStatuses, FileManagerType type, bool recursive);
    Task<List<FileSystemEntryDto>> GetEntriesByIds(List<string> ids, Guid archiveId, EntryKind? kind, FileManagerType type);
    Task<FileSystemEntryDto> GetEntryById(string id, Guid archiveId, FileManagerType type);
    Task<List<HierarchyFolderDto>> GetEntryParentsById(string id, Guid archiveId, FileManagerType type);
    Task<FileSystemEntryDto> GetRootEntry(Guid archiveId, FileManagerType type);
    Task<List<FileSystemEntryDto>> SearchEntries(string search, Guid archiveId, EntryKind? kind, FileManagerType type);

    // Unified mutation methods
    Task<FileSystemEntryDto> CreateEntry(CreateEntryDto dto, Guid archiveId, FileManagerType type);
    Task<FileSystemEntryDto> RenameEntry(RenameEntryDto dto, Guid archiveId, FileManagerType type);
    Task<bool> MoveEntry(MoveEntryDto dto, Guid archiveId, FileManagerType type);
    Task<bool> MoveAllEntries(MoveAllEntriesDto dto, Guid archiveId, FileManagerType type);
    Task<bool> CopyEntry(CopyEntryDto dto, Guid archiveId, FileManagerType type);
    Task<bool> DeleteEntry(string id, Guid archiveId, FileManagerType type);

    // File-content operations (file-only)
    Task<string> GetFileToken(string id, bool isVersion, Guid archiveId, FileManagerType type);
    Task<bool> DeleteFileToken(Guid token, Guid archiveId, FileManagerType type);
    Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion, Guid archiveId, FileManagerType type);
    Task<FileSourceDto> FileViewer(string id, Guid archiveId, FileManagerType type);
    Task<byte[]> DownloadFile(string id, bool isVersion, Guid archiveId, FileManagerType type);
    Task<byte[]> DownloadAll(DownloadAllDto downloadAllDto);
    Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType type);
    Task<List<string>> ReadTextFile(string id, bool isVersion, Guid archiveId, FileManagerType type);
    Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion, Guid archiveId, FileManagerType type);
    Task<List<FileSystemEntryDto>> UploadFiles(FileUploadDto fileUploadDto);

    // Additional query methods
    Task<List<FileSystemEntryDto>> GetEntries(EntryFilterDto entryFilterDto);
    Task<List<FileSystemEntryDto>> GetEntryHistory(string id, Guid archiveId, FileManagerType type);
    Task<bool> RestoreEntry(string id, Guid archiveId, FileManagerType type);

  }
}
