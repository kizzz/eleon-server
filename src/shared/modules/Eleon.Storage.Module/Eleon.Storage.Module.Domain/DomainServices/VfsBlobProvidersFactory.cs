using Eleon.Storage.Lib.Constants;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Models;
using SharedModule.modules.Blob.Module.S3BlobProviders;
using SharedModule.modules.Blob.Module.Shared;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharedModule.modules.Blob.Module;
public static class VfsBlobProvidersFactory
{
  public static IVfsBlobProvider Create(StorageProviderDto StorageProviderDto, Guid? tenantId)
  {
    if (!StorageProviderDto.IsActive)
    {
      throw new InvalidOperationException("Cannot create a VFS Blob Provider from an inactive Storage Provider DTO. In order to create a VFS Blob Provider, the Storage Provider DTO must be inactive.");
    }

    return StorageProviderDto.StorageProviderTypeName switch
    {
      StorageProviderDomainConstants.StorageTypeFileSystem => new FileSystemVfsBlobProvider(StorageProviderDto.Settings, tenantId),
      StorageProviderDomainConstants.StorageTypeSFTP => new SftpVfsBlobProvider(StorageProviderDto.Settings, tenantId),
      _ => throw new NotSupportedException($"Unknown provider: {StorageProviderDto.StorageProviderTypeName}")
    };
  }
}
