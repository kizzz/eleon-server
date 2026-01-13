using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.VfsShared;
using EleonsoftSdk.modules.Helpers.Module;
using Renci.SshNet;
using SharedModule.modules.Blob.Module.CustomBlobProviders.SFTP;
using SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;
using SharedModule.modules.Blob.Module.Models;
using SharedModule.modules.Blob.Module.Shared;
using SharedModule.modules.Blob.Module.VfsShared;
using Volo.Abp.BlobStoring;

namespace SharedModule.modules.Blob.Module.S3BlobProviders;
public class SftpVfsBlobProvider : IVfsBlobProvider
{
  // Forward to original ABP methods
  private readonly SftpBlobProvider _inner;
  private readonly BlobContainerConfiguration _containerConfiguration;
  private readonly IBlobSftpPathCalculator PathCalculator;

  public SftpVfsBlobProvider(IList<StorageProviderSettingDto> settings, Guid? tenantId)
  {
    var sftpPathCalculator = new DefaultSftpBlobPathCalculator();
    _inner = new SftpBlobProvider(sftpPathCalculator);
    PathCalculator = sftpPathCalculator;
    _containerConfiguration = new BlobContainerConfiguration();

    foreach (var setting in settings)
    {
      _containerConfiguration.SetConfiguration($"Sftp.{setting.Key}", setting.Value);
    }

    _containerConfiguration.SetConfiguration("TenantId", tenantId.HasValue ? tenantId.Value.ToString("D") : string.Empty);
  }

  public async Task SaveAsync(VfsSaveArgs args)
  {
    var parsedArgs = new BlobProviderSaveArgs(
        args.ContainerName,
        _containerConfiguration,
        args.BlobName,
        args.BlobStream,
        args.OverrideExisting);

    var cfg = _containerConfiguration.GetSftpConfiguration();
    var path = PathCalculator.Calculate(
        new BlobProviderGetArgs(args.ContainerName, _containerConfiguration, args.BlobName)
    );
    var tenantId = _containerConfiguration.GetConfiguration<string>("TenantId");

    var client = SftpClientManager.GetOrCreateClient(cfg, tenantId);

    var directory = Path.GetDirectoryName(path)?.Replace("\\", "/");
    if (!string.IsNullOrEmpty(directory))
    {
      EnsureDirectories(client, directory);
    }

    if (args.IsFolder)
    {
      if (!client.Exists(path))
      {
        client.CreateDirectory(path);
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
    return await _inner.ExistsAsync(parsedArgs);
  }

  public async Task<Stream?> GetOrNullAsync(VfsGetArgs args)
  {
    var parsedArgs = new BlobProviderGetArgs(
        args.ContainerName,
        _containerConfiguration,
        args.BlobName);

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

  public async Task<IReadOnlyList<VfsFileInfo>> ListAsync(VfsListArgs args)
  {
    var parsedArgs = new BlobProviderGetArgs(args.ContainerName, _containerConfiguration, args.BlobName ?? "dummy");
    var cfg = parsedArgs.Configuration.GetSftpConfiguration();
    var tenantId = _containerConfiguration.GetConfiguration<string>("TenantId");
    var client = SftpClientManager.GetOrCreateClient(cfg, tenantId);

    var folder = PathCalculator.Calculate(parsedArgs);
    folder = folder.Replace("\\", "/");

    // Remove the trailing dummy part if any
    if (args.BlobName == null)
    {
      var lastSlash = folder.LastIndexOf('/');
      if (lastSlash > 0)
        folder = folder[..lastSlash];
    }

    if (!client.Exists(folder))
    {
      return Array.Empty<VfsFileInfo>();
    }

    var result = new List<VfsFileInfo>();

    await Task.Run(() =>
    {
      if (args.IsRecursiveSearch)
      {
        CollectFilesRecursive(client, folder, folder, result);
      }
      else
      {
        // Flat listing (current directory only)
        foreach (var entry in client.ListDirectory(folder))
        {
          if (entry.Name == "." || entry.Name == "..")
            continue;

          if (entry.IsRegularFile)
          {
            result.Add(new VfsFileInfo
            {
              Key = entry.Name.Replace("\\", "/"),
              Size = entry.Attributes.Size,
              LastModified = entry.Attributes.LastWriteTimeUtc
            });
          }
          else if (entry.IsDirectory)
          {
            result.Add(new VfsFileInfo
            {
              Key = entry.Name.Replace("\\", "/"),
              Size = 0,
              IsFolder = true,
              LastModified = entry.Attributes.LastWriteTimeUtc
            });
          }

        }
      }
    });

    return result.Take(100).ToList();
  }

  private void CollectFilesRecursive(SftpClient client, string rootPath, string currentDir, List<VfsFileInfo> results)
  {
    foreach (var entry in client.ListDirectory(currentDir))
    {
      if (entry.Name == "." || entry.Name == "..")
      {
        continue;
      }

      if (entry.IsDirectory)
      {
        results.Add(new VfsFileInfo
        {
          Key = entry.FullName.StartsWith(rootPath)
              ? entry.FullName.Substring(rootPath.Length).TrimStart('/').Replace("\\", "/")
              : entry.Name.Replace("\\", "/"),
          Size = 0,
          IsFolder = true,
          LastModified = entry.Attributes.LastWriteTimeUtc
        });
        CollectFilesRecursive(client, rootPath, entry.FullName, results);
      }
      else if (entry.IsRegularFile)
      {
        var relative = entry.FullName.StartsWith(rootPath)
            ? entry.FullName.Substring(rootPath.Length).TrimStart('/')
            : entry.Name;

        results.Add(new VfsFileInfo
        {
          Key = relative.Replace("\\", "/"),
          Size = entry.Attributes.Size,
          LastModified = entry.Attributes.LastWriteTimeUtc
        });
      }
    }
  }

  private void EnsureDirectories(SftpClient client, string dir)
  {
    var parts = dir.Split('/', StringSplitOptions.RemoveEmptyEntries);
    var current = "";
    foreach (var part in parts)
    {
      current += "/" + part;
      if (!client.Exists(current))
        client.CreateDirectory(current);
    }
  }

  public void Dispose()
  {
    _inner.Dispose();
    SftpClientManager.Dispose();
  }

  public async Task<VfsPagedResult> ListPagedAsync(VfsListPagedArgs args)
  {
    var parsedArgs = new BlobProviderGetArgs(args.ContainerName, _containerConfiguration, args.BlobName ?? "dummy");
    var cfg = parsedArgs.Configuration.GetSftpConfiguration();
    var tenantId = _containerConfiguration.GetConfiguration<string>("TenantId");
    var client = SftpClientManager.GetOrCreateClient(cfg, tenantId);

    var folder = PathCalculator.Calculate(parsedArgs);
    folder = folder.Replace("\\", "/");

    // Remove the trailing dummy part if any
    if (args.BlobName == null)
    {
      var lastSlash = folder.LastIndexOf('/');
      if (lastSlash > 0)
        folder = folder[..lastSlash];
    }

    if (!client.Exists(folder))
    {
      return new VfsPagedResult(Array.Empty<VfsFileInfo>(), 0);
    }

    var result = new List<VfsFileInfo>();

    await Task.Run(() =>
    {
      if (args.IsRecursiveSearch)
      {
        CollectFilesRecursive(client, folder, folder, result);
      }
      else
      {
        // Flat listing (current directory only) - already enumerable via ListDirectory
        foreach (var entry in client.ListDirectory(folder))
        {
          if (entry.Name == "." || entry.Name == "..")
            continue;

          if (entry.IsRegularFile)
          {
            result.Add(new VfsFileInfo
            {
              Key = entry.Name.Replace("\\", "/"),
              Size = entry.Attributes.Size,
              LastModified = entry.Attributes.LastWriteTimeUtc
            });
          }
          else if (entry.IsDirectory)
          {
            result.Add(new VfsFileInfo
            {
              Key = entry.Name.Replace("\\", "/"),
              Size = 0,
              IsFolder = true,
              LastModified = entry.Attributes.LastWriteTimeUtc
            });
          }
        }
      }
    });

    // Apply pagination - for recursive search, we've already collected everything
    // For flat listing, we could optimize further but SFTP ListDirectory is already efficient
    var pagedItems = result.Skip(args.SkipResults).Take(args.MaxResults).ToList();
    
    // Total count: if we got fewer items than requested, we know the exact count
    // Otherwise, we don't know without collecting everything (already done for recursive)
    var totalCount = pagedItems.Count < args.MaxResults 
        ? args.SkipResults + pagedItems.Count 
        : (args.IsRecursiveSearch ? result.Count : -1); // For recursive, we have exact count

    return new VfsPagedResult(pagedItems, totalCount);
  }
}
