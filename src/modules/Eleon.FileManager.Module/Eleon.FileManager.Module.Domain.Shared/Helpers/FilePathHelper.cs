using SharedModule.modules.Blob.Module.Shared;
using SharedModule.modules.Blob.Module.VfsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.FileManager.Module.Entities;

namespace VPortal.FileManager.Module.Helpers
{
  public static class FilePathHelper
  {
    public static string GetContainerName(Guid storageProviderId)
    {
      var id = storageProviderId.ToString();
      return string.IsNullOrWhiteSpace(id) ? "default" : id;
    }

    public static string NormalizePath(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        return "./";

      if (!path.StartsWith("./"))
        path = $"./{path}";

      path = path.Replace('\\', '/');
      path = path.Replace("/./", "/");
      while (path.Contains("//"))
        path = path.Replace("//", "/");

      return path;
    }

    public static string CombinePaths(string basePath, string relative)
    {
      basePath = NormalizePath(basePath);
      relative = NormalizePath(relative);

      if (string.IsNullOrEmpty(basePath))
        return relative;

      if (string.IsNullOrEmpty(relative))
        return basePath;

      return $"{basePath.TrimEnd('/')}/{relative.TrimStart('/')}";
    }

    public static string GetFileNameFromKey(string key)
    {
      key = NormalizePath(key);
      if (string.IsNullOrEmpty(key))
        return string.Empty;

      var parts = key.Split('/');
      return parts.Last();
    }

    public static string GetParentKey(string key)
    {
      key = NormalizePath(key);
      var idx = key.LastIndexOf('/');
      if (idx <= 0)
        return string.Empty;

      var parent = key[..idx];
      if (parent == ".")
        return "/";

      return parent;
    }

    public static Stream ToStream(byte[] bytes)
    {
      return new MemoryStream(bytes);
    }

    public static Stream ToStream(string? source)
    {
      if (string.IsNullOrEmpty(source))
        return new MemoryStream();

      try
      {
        return new MemoryStream(Convert.FromBase64String(source));
      }
      catch
      {
        return new MemoryStream(Encoding.UTF8.GetBytes(source));
      }
    }

    private static string NormalizeArgsPath(FileArchiveEntity archive, string userId, string path)
    {
      var rootPath = NormalizePath(archive.PhysicalRootFolderId);

      if (archive.IsPersonalizedArchive)
      {
        rootPath = CombinePaths(rootPath, userId);
      }

      var normalizedPath = NormalizePath(path);
      return CombinePaths(rootPath, normalizedPath);


    }
    public static VfsSaveArgs CreateSaveArgs(FileArchiveEntity archive, string userId, string key, Stream stream, bool isFolder = false)
    {
      return new VfsSaveArgs(GetContainerName(archive.StorageProviderId), NormalizeArgsPath(archive, userId, key), stream, isFolder: isFolder);
    }

    public static VfsGetArgs CreateGetArgs(FileArchiveEntity archive, string userId, string key)
    {
      return new VfsGetArgs(GetContainerName(archive.StorageProviderId), NormalizeArgsPath(archive, userId, key));
    }

    public static VfsListArgs CreateListArgs(FileArchiveEntity archive, string userId, string folder)
    {
      return new VfsListArgs(GetContainerName(archive.StorageProviderId), NormalizeArgsPath(archive, userId, folder));
    }

    public static VfsListPagedArgs CreateListPagedArgs(FileArchiveEntity archive, string userId, string folder, int skipCount, int maxResultCount, bool isRecursiveSearch = false)
    {
      var args = new VfsListPagedArgs(GetContainerName(archive.StorageProviderId), NormalizeArgsPath(archive, userId, folder), isRecursiveSearch);
      args.SkipResults = skipCount;
      args.MaxResults = maxResultCount;
      return args;
    }

    public static VfsDeleteArgs CreateDeleteArgs(FileArchiveEntity archive, string userId, string key)
    {
      return new VfsDeleteArgs(GetContainerName(archive.StorageProviderId), NormalizeArgsPath(archive, userId, key));
    }

    public static VfsExistArgs CreateExistArgs(FileArchiveEntity archive, string userId, string key)
    {
      return new VfsExistArgs(GetContainerName(archive.StorageProviderId), NormalizeArgsPath(archive, userId, key));
    }
  }
}
