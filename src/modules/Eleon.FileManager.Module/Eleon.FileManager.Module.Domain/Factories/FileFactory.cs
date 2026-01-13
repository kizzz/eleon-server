using Common.EventBus.Module;
using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using EleonsoftSdk.modules.StorageProvider.Module;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.modules.Blob.Module;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.Repositories.File;
using VPortal.Storage.Module.DomainServices;

namespace VPortal.FileManager.Module.Factories
{
  public class FileFactory : IFileFactory, ITransientDependency
  {
    private readonly IServiceProvider serviceProvider;
    private readonly VfsStorageProviderCacheManager vfsStorageProviderCacheService;
    private readonly ICurrentTenant currentTenant;
    private readonly ICurrentUser currentUser;
    private readonly IDistributedEventBus _eventBus;

    public FileFactory(
        IServiceProvider serviceProvider,
        VfsStorageProviderCacheManager vfsStorageProviderCacheService,
        ICurrentTenant currentTenant,
        ICurrentUser currentUser,
        IDistributedEventBus eventBus)
    {
      this.serviceProvider = serviceProvider;
      this.vfsStorageProviderCacheService = vfsStorageProviderCacheService;
      this.currentTenant = currentTenant;
      this.currentUser = currentUser;
      _eventBus = eventBus;
    }

    public async Task<IFileSystemEntryRepository> Get(FileArchiveEntity fileArchiveEntity, FileManagerType type)
    {
      IFileSystemEntryRepository result = null;
      if (fileArchiveEntity.FileArchiveHierarchyType == FileArchiveHierarchyType.Physical || type == FileManagerType.Provider)
      {
        var vfsProvider = await vfsStorageProviderCacheService.ResolveProviderAsync(currentTenant.Id, fileArchiveEntity.StorageProviderId.ToString());
        result = new FileStorageProviderRepository(vfsProvider, currentUser);
      }
      else if (type == FileManagerType.FileArchive)
      {

        if (fileArchiveEntity.FileArchiveHierarchyType == FileArchiveHierarchyType.Physical)
        {
          var response = await _eventBus.RequestAsync<GetStorageProviderResponseMsg>(new GetStorageProviderMsg { StorageProviderId = fileArchiveEntity.StorageProviderId.ToString() });
          var storageProvider = response.StorageProvider;
          var proxySetting = storageProvider.Settings.Where(s => s.Key == "ProxyId").FirstOrDefault();
          if (proxySetting != null && proxySetting.Value != null)
          {
            //result = await proxyAppServiceProvider.ResolveScopedProxyAppService<IFileRemoteAppService>(Guid.Parse(proxySetting.Value));
            throw new NotImplementedException("Proxy Management Module was moved to erp.");
          }
        }
        else
        {
          result = Get(fileArchiveEntity.FileArchiveHierarchyType);
        }
      }

      if (result != null)
      {
        result.Archive = fileArchiveEntity;
      }

      return result;
    }

    public IFileSystemEntryRepository Get(FileArchiveHierarchyType fileArchiveHierarchyType) =>
        fileArchiveHierarchyType switch
        {
          FileArchiveHierarchyType.Physical => serviceProvider.GetService<FilePhysicalRepository>(),
          FileArchiveHierarchyType.Virtual => serviceProvider.GetService<FileVirtualRepository>(), /*serviceProvider.GetService<FilePhysicalRepository>(),*/
          _ => throw new ArgumentException($"Unknown FileArchiveHierarchyType: {fileArchiveHierarchyType}")
        };
  }
}
