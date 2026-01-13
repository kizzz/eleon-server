using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Repositories.File
{
  public class FileAttachmentRepository : IFileRepository
  {
    public FileArchiveEntity Archive { get; set; }

    public Task<bool> CopyFile(string fileId, string destinationFolderId)
    {
      throw new NotImplementedException();
    }

    public Task<FileSystemEntry> CreateFolder(string name, string folderId, string parentId)
    {
      throw new NotImplementedException();
    }

    public Task<bool> DeleteFile(string id)
    {
      throw new NotImplementedException();
    }

    public Task<bool> DeleteFileToken(Guid token)
    {
      throw new NotImplementedException();
    }

    public Task<bool> DeleteFolder(string id)
    {
      throw new NotImplementedException();
    }

    public Task<byte[]> DownloadFile(string id, bool isVersion)
    {
      throw new NotImplementedException();
    }

    public Task<byte[]> DownloadFileByToken(string id, string token, bool isVersion)
    {
      throw new NotImplementedException();
    }

    public Task<FileSourceValueObject> FileViewer(string id)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> GetFilesByFolderId(string id)
    {
      throw new NotImplementedException();
    }

    public Task<FileSystemEntry> GetFileById(string id)
    {
      throw new NotImplementedException();
    }

    public Task<byte[]> GetFileByToken(string id, Guid token, bool isVersion)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> GetFiles(List<string> ids)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetFileToken(string id, bool isVersion)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> GetFolderChildsById(string id)
    {
      throw new NotImplementedException();
    }

    public Task<FileSystemEntry> GetFolderDetailById(string id)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> GetFolderParentsById(string id)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> GetFolders(List<string> ids)
    {
      throw new NotImplementedException();
    }

    public Task<FileSystemEntry> GetRootFolder()
    {
      throw new NotImplementedException();
    }

    public Task<bool> MoveAllFile(List<string> fileIds, List<string> folders, string destinationFolderId)
    {
      throw new NotImplementedException();
    }

    public Task<bool> MoveFile(string fileId, string destinationFolderId)
    {
      throw new NotImplementedException();
    }

    public Task<List<string>> ReadTextFile(string id, bool isVersion)
    {
      throw new NotImplementedException();
    }

    public Task<List<string>> ReadTextFileByToken(string id, string token, bool isVersion)
    {
      throw new NotImplementedException();
    }

    public Task<bool> RenameFile(string id, string name)
    {
      throw new NotImplementedException();
    }

    public Task<FileSystemEntry> RenameFolder(string id, string name)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> SearchFile(string searchString)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> SearchFolder(string searchString)
    {
      throw new NotImplementedException();
    }

    public Task<List<FileSystemEntry>> UploadFiles(List<FileSourceValueObject> filesToUpload, string folderId)
    {
      throw new NotImplementedException();
    }

    public Task<FileSystemEntry> UploadNewVersion(string oldFileId, MemoryStream newFileData)
    {
      throw new NotImplementedException();
    }
  }
}
