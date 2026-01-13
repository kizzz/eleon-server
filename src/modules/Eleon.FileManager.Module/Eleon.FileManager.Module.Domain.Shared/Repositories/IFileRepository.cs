using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Repositories
{
  public interface IFileRepository
  {
    FileArchiveEntity Archive { get; set; }
    Task<List<FileSystemEntry>> GetFilesByFolderId(string id);
    Task<List<FileSystemEntry>> GetFiles(List<string> ids);
    Task<List<FileSystemEntry>> GetFolders(List<string> ids);
    Task<string> GetFileToken(string id, bool isVersion);
    Task<bool> DeleteFileToken(Guid token);
    Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion);
    Task<bool> MoveFile(string fileId, string destinationFolderId);
    Task<bool> MoveAllFile(List<string> fileIds, List<string> folders, string destinationFolderId);
    Task<bool> CopyFile(string fileId, string destinationFolderId);
    Task<bool> DeleteFile(string id);
    Task<FileSourceValueObject> FileViewer(string id);
    Task<byte[]> DownloadFile(string id, bool isVersion);
    Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion);
    Task<List<string>> ReadTextFile(string id, bool isVersion);
    Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion);
    Task<List<FileSystemEntry>> SearchFile(string searchString);
    Task<bool> RenameFile(string id, string name);
    Task<FileSystemEntry> CreateFolder(string name, string folderId, string parentId);
    Task<bool> DeleteFolder(string id);
    Task<List<FileSystemEntry>> GetFolderChildsById(string id);
    Task<FileSystemEntry> GetFolderDetailById(string id);
    Task<List<FileSystemEntry>> GetFolderParentsById(string id);
    Task<FileSystemEntry> GetRootFolder();
    Task<FileSystemEntry> RenameFolder(string id, string name);
    Task<List<FileSystemEntry>> SearchFolder(string searchString);
    Task<List<FileSystemEntry>> UploadFiles(List<FileSourceValueObject> filesToUpload, string folderId);
    Task<FileSystemEntry> UploadNewVersion(string oldFileId, MemoryStream newFileData);
    Task<FileSystemEntry> GetFileById(string id);
  }
}
