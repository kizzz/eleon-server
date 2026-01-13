using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Repositories
{
  public interface IFileSystemEntryRepository
  {
    FileArchiveEntity Archive { get; set; }

    // Unified query methods
    Task<List<FileSystemEntry>> GetEntriesByParentId(string parentId, EntryKind? kind = null, bool recursive = false);
    Task<(List<FileSystemEntry> Items, long TotalCount)> GetEntriesByParentIdPaged(string parentId, EntryKind? kind, int skipCount, int maxResultCount, string? sorting = null, bool recursive = false);
    Task<List<FileSystemEntry>> GetEntriesByIds(List<string> ids, EntryKind? kind = null);
    Task<FileSystemEntry> GetEntryById(string id);
    Task<List<FileSystemEntry>> GetEntryParentsById(string id);
    Task<FileSystemEntry> GetRootEntry();
    Task<List<FileSystemEntry>> SearchEntries(string search, EntryKind? kind = null);

    // Unified mutation methods
    Task<FileSystemEntry> CreateEntry(
        EntryKind kind,
        string name,
        string? parentId,
        string? physicalFolderId = null,
        bool isShared = false,
        string? extension = null,
        string? path = null,
        string? size = null,
        string? thumbnailPath = null);
    Task<FileSystemEntry> RenameEntry(string id, string name);
    Task<bool> MoveEntry(string entryId, string destinationParentId);
    Task<bool> MoveAllEntries(List<string> entryIds, string destinationParentId);
    Task<bool> CopyEntry(string entryId, string destinationParentId);
    Task<bool> DeleteEntry(string id);

    // File-content operations (file-only, must validate EntryKind == File)
    Task<string> GetFileToken(string id, bool isVersion);
    Task<bool> DeleteFileToken(Guid token);
    Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion);
    Task<FileSourceValueObject> FileViewer(string id);
    Task<byte[]> DownloadFile(string id, bool isVersion);
    Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion);
    Task<List<string>> ReadTextFile(string id, bool isVersion);
    Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion);
    Task<List<FileSystemEntry>> UploadFiles(List<FileSourceValueObject> filesToUpload, string folderId = null);
    Task<FileSystemEntry> UploadNewVersion(string oldFileId, MemoryStream newFileData);
  }
}

