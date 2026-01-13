using Eleon.Storage.Lib.CustomBlobProviders.FileSystem;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.VfsShared;
using EleonsoftSdk.modules.Helpers.Module;
using SharedModule.modules.Blob.Module.Models;
using SharedModule.modules.Blob.Module.Shared;
using SharedModule.modules.Blob.Module.VfsShared;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;

namespace SharedModule.modules.Blob.Module.S3BlobProviders;
public class FileSystemVfsBlobProvider : IVfsBlobProvider
{
  private readonly FileSystemBlobProvider _inner;
  private readonly IBlobFilePathCalculator _filePathCalculator;

  private readonly BlobContainerConfiguration _containerConfiguration;

  public FileSystemVfsBlobProvider(IList<StorageProviderSettingDto> settings, Guid? tenantId)
  {
    var pathCalculator = new EleonBlobFilePathCalculator();
    _inner = new EleonFileSystemBlobProvider(pathCalculator);
    _filePathCalculator = pathCalculator;

    _containerConfiguration = new BlobContainerConfiguration();

    foreach (var setting in settings)
    {
      _containerConfiguration.SetConfiguration($"FileSystem.{setting.Key}", setting.Value);
    }

    _containerConfiguration.SetConfiguration("TenantId", tenantId.HasValue ? tenantId.Value.ToString("D") : string.Empty);
  }

  // Forward to original ABP methods
  public async Task SaveAsync(VfsSaveArgs args)
  {
    var parsedArgs = new BlobProviderSaveArgs(
        args.ContainerName,
        _containerConfiguration,
        args.BlobName,
        args.BlobStream,
        args.OverrideExisting);

    var path = _filePathCalculator.Calculate(
        new BlobProviderGetArgs(args.ContainerName, _containerConfiguration, args.BlobName)
    );

    var directory = Path.GetDirectoryName(path);
    if (!string.IsNullOrEmpty(directory))
    {
      Directory.CreateDirectory(directory);  // Ensure directories exist
    }

    if (args.IsFolder)
    {
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
    }
    else
    {
      await _inner.SaveAsync(parsedArgs);
    }
  }


  public async Task<bool> DeleteAsync(VfsDeleteArgs args)
  {
    var parsedArgs = new BlobProviderDeleteArgs(
        args.ContainerName,
        _containerConfiguration,
        args.BlobName);

    return await _inner.DeleteAsync(parsedArgs);
  }

  public async Task<bool> ExistsAsync(VfsExistArgs args)
  {
    var parsedArgs = new BlobProviderExistsArgs(
        args.ContainerName,
        _containerConfiguration,
        args.BlobName);
    if (await _inner.ExistsAsync(parsedArgs))
    {
      return true;
    }

    var path = _filePathCalculator.Calculate(
        new BlobProviderGetArgs(args.ContainerName, _containerConfiguration, args.BlobName));
    return Directory.Exists(path);
  }

  public async Task<Stream?> GetOrNullAsync(VfsGetArgs args)
  {
    var parsedArgs = new BlobProviderGetArgs(
        args.ContainerName,
        _containerConfiguration,
        args.BlobName);

    var test = _filePathCalculator.Calculate(parsedArgs);

    return await _inner.GetOrNullAsync(parsedArgs);
  }

  public async Task<byte[]?> GetAllBytesOrNullAsync(VfsGetArgs args)
  {
    var stream = await GetOrNullAsync(args);

    if (stream == null)
    {
      return null;
    }

    using (stream)
    {
      return await stream.GetAllBytesAsync();
    }
  }
  public async Task<bool> TestAsync(VfsTestArgs args)
  {
    var parsedSaveArgs = new BlobProviderSaveArgs(
        args.ContainerName,
        _containerConfiguration,
        args.BlobName,
        args.BlobStream);
    var parsedDeleteArgs = new BlobProviderDeleteArgs(
        args.ContainerName,
        _containerConfiguration,
        args.BlobName);

    await _inner.SaveAsync(parsedSaveArgs);
    return await _inner.DeleteAsync(parsedDeleteArgs);
  }

  // Extended method
  public async Task<IReadOnlyList<VfsFileInfo>> ListAsync(VfsListArgs args)
  {
    var (searchRoot, prefixFilter) = ResolveSearchRoot(args.ContainerName, args.BlobName);

    if (!Directory.Exists(searchRoot))
    {
      return Array.Empty<VfsFileInfo>();
    }
    var searchOption = args.IsRecursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    List<VfsFileInfo> result = [];
    const int maxResults = 100;

    // Use EnumerateFiles/EnumerateDirectories for streaming instead of loading everything into memory
    var directories = Directory.EnumerateDirectories(searchRoot, "*", searchOption)
           .Select(d =>
           {
             var info = new DirectoryInfo(d);
             return new VfsFileInfo
             {
               Key = Path.GetRelativePath(searchRoot, d),
               Size = 0,
               IsFolder = true,
               LastModified = info.LastWriteTimeUtc
             };
           })
           .Take(maxResults);

    var files = Directory.EnumerateFiles(searchRoot, "*", searchOption)
        .Select(f =>
        {
          var info = new FileInfo(f);
          return new VfsFileInfo
          {
            Key = Path.GetRelativePath(searchRoot, f),
            Size = info.Length,
            IsFolder = false,
            LastModified = info.LastWriteTimeUtc
          };
        })
        .Take(maxResults - result.Count);

    if (!string.IsNullOrEmpty(prefixFilter))
    {
      directories = directories.Where(d => d.Key.StartsWith(prefixFilter, StringComparison.OrdinalIgnoreCase));
      files = files.Where(f => f.Key.StartsWith(prefixFilter, StringComparison.OrdinalIgnoreCase));
    }

    result.AddRange(directories);
    result.AddRange(files);

    return result.Take(maxResults).ToList();
  }

  public async Task<VfsPagedResult> ListPagedAsync(VfsListPagedArgs args)
  {
    var (searchRoot, prefixFilter) = ResolveSearchRoot(args.ContainerName, args.BlobName);

    if (!Directory.Exists(searchRoot))
    {
      return new VfsPagedResult(Array.Empty<VfsFileInfo>(), 0);
    }
    var searchOption = args.IsRecursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

    // Use EnumerateFiles/EnumerateDirectories for streaming instead of loading everything into memory
    // For paging, we stream and apply Skip/Take before materializing
    var directories = Directory.EnumerateDirectories(searchRoot, "*", searchOption)
           .Select(d =>
           {
             var info = new DirectoryInfo(d);
             return new VfsFileInfo
             {
               Key = Path.GetRelativePath(searchRoot, d),
               Size = 0,
               IsFolder = true,
               LastModified = info.LastWriteTimeUtc
             };
           });

    var files = Directory.EnumerateFiles(searchRoot, "*", searchOption)
        .Select(f =>
        {
          var info = new FileInfo(f);
          return new VfsFileInfo
          {
            Key = Path.GetRelativePath(searchRoot, f),
            Size = info.Length,
            IsFolder = false,
            LastModified = info.LastWriteTimeUtc
          };
        });

    // Combine and apply pagination - materialize only what we need
    var allItems = directories.Concat(files);
    if (!string.IsNullOrEmpty(prefixFilter))
    {
      allItems = allItems.Where(i => i.Key.StartsWith(prefixFilter, StringComparison.OrdinalIgnoreCase));
    }

    var allItemsList = allItems.ToList();
    var pagedItems = allItemsList.Skip(args.SkipResults).Take(args.MaxResults).ToList();
    var totalCount = allItemsList.Count;

    return new VfsPagedResult(pagedItems, totalCount);
  }

  public void Dispose()
  {
    // No dispose for filesystem
  }

  private (string SearchRoot, string? PrefixFilter) ResolveSearchRoot(string containerName, string prefix)
  {
    var root = GetContainerRootPath(containerName);
    if (string.IsNullOrWhiteSpace(prefix))
    {
      return (root, null);
    }

    var normalizedPrefix = prefix.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
    var directPath = Path.Combine(root, normalizedPrefix);
    if (Directory.Exists(directPath))
    {
      return (directPath, null);
    }

    var dirPart = Path.GetDirectoryName(normalizedPrefix);
    var namePrefix = Path.GetFileName(normalizedPrefix);
    var searchRoot = string.IsNullOrEmpty(dirPart) ? root : Path.Combine(root, dirPart);
    return (searchRoot, namePrefix);
  }

  private string GetContainerRootPath(string containerName)
  {
    var dummyPath = _filePathCalculator.Calculate(
        new BlobProviderGetArgs(containerName, _containerConfiguration, "__container_root__"));
    return Path.GetDirectoryName(dummyPath) ?? dummyPath;
  }
}
