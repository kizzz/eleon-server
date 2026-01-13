using Renci.SshNet;
using SharedModule.modules.Blob.Module.CustomBlobProviders.SFTP;
using System;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;

namespace SharedModule.modules.Blob.Module.CustomStorageProviders.SFTP;

public class SftpBlobProvider : BlobProviderBase, ITransientDependency, IDisposable
{
  protected IBlobSftpPathCalculator PathCalculator { get; }


  public SftpBlobProvider(IBlobSftpPathCalculator sftpPathCalculator)
  {
    PathCalculator = sftpPathCalculator;
  }

  public override async Task SaveAsync(BlobProviderSaveArgs args)
  {
    var cfg = args.Configuration.GetSftpConfiguration();
    var path = PathCalculator.Calculate(args);
    var tenantId = args.Configuration.GetConfiguration<string>("TenantId");

    var client = SftpClientManager.GetOrCreateClient(cfg, tenantId);
    var dir = Path.GetDirectoryName(path)?.Replace("\\", "/");
    if (dir != null)
    {
      EnsureDirectories(client, dir);
    }

    if (client.Exists(path) && !args.OverrideExisting)
    {
      throw new BlobAlreadyExistsException($"BLOB '{args.BlobName}' already exists in '{args.ContainerName}'.");
    }

    using (var ms = new MemoryStream())
    {
      await args.BlobStream.CopyToAsync(ms, args.CancellationToken);
      ms.Position = 0;
      client.UploadFile(ms, path, args.OverrideExisting);
    }
  }

  public override Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
  {
    var cfg = args.Configuration.GetSftpConfiguration();
    var path = PathCalculator.Calculate(args);
    var tenantId = args.Configuration.GetConfiguration<string>("TenantId");
    var client = SftpClientManager.GetOrCreateClient(cfg, tenantId);

    if (client.Exists(path))
    {
      client.DeleteFile(path);
      return Task.FromResult(true);
    }

    return Task.FromResult(false);
  }

  public override Task<bool> ExistsAsync(BlobProviderExistsArgs args)
  {
    var cfg = args.Configuration.GetSftpConfiguration();
    var path = PathCalculator.Calculate(args);
    var tenantId = args.Configuration.GetConfiguration<string>("TenantId");

    var client = SftpClientManager.GetOrCreateClient(cfg, tenantId);
    var exists = client.Exists(path);
    return Task.FromResult(exists);
  }

  public override Task<Stream?> GetOrNullAsync(BlobProviderGetArgs args)
  {
    var cfg = args.Configuration.GetSftpConfiguration();
    var path = PathCalculator.Calculate(args);
    var tenantId = args.Configuration.GetConfiguration<string>("TenantId");

    var client = SftpClientManager.GetOrCreateClient(cfg, tenantId);

    if (!client.Exists(path))
    {
      return Task.FromResult<Stream?>(null);
    }

    var ms = new MemoryStream();
    client.DownloadFile(path, ms);
    ms.Position = 0;

    return Task.FromResult<Stream?>(ms);
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
    SftpClientManager.Dispose();
  }
}
