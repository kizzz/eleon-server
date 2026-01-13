using Common.EventBus.Module;
using Common.Module.Constants;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Messages;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Logging.Module;
using Microsoft.Extensions.Logging;
using SharedModule.modules.Blob.Module.Models;
using SharedModule.modules.Helpers.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;
using VPortal.Storage.Module.DomainServices;

namespace VPortal.FileManager.Module.DomainServices
{
  public class FileArchiveDomainService : DomainService
  {
    private readonly IVportalLogger<FileArchiveDomainService> logger;
    private readonly IVirtualFolderRepository virtualFolderRepository;
    private readonly IArchiveRepository archiveRepository;
    private readonly FileArchivePermissionCheckerDomainService permissionManagement;
    private readonly IDistributedEventBus _eventBus;

    public FileArchiveDomainService(
        IVportalLogger<FileArchiveDomainService> logger,
        IVirtualFolderRepository virtualFolderRepository,
        FileArchivePermissionCheckerDomainService permissionManagement,
        IArchiveRepository archiveRepository,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.permissionManagement = permissionManagement;
      this.virtualFolderRepository = virtualFolderRepository;
      this.archiveRepository = archiveRepository;
      _eventBus = eventBus;
    }

    public async Task<FileArchiveEntity> CreateFileArchive(
        string name,
        FileArchiveHierarchyType fileArchiveHierarchyType,
        Guid storageProviderId,
        bool isActive,
        bool isPersonalizedArchive,
        string physicalRootFolderId)
    {
      FileArchiveEntity result = default;
      try
      {
        var entityId = GuidGenerator.Create();
        var fileArchiveEntity = new FileArchiveEntity(entityId)
        {
          Name = name,
          FileArchiveHierarchyType = fileArchiveHierarchyType,
          StorageProviderId = storageProviderId,
          IsActive = isActive,
          IsPersonalizedArchive = isPersonalizedArchive,
          RootFolderId = "./",
          PhysicalRootFolderId = physicalRootFolderId
        };

        if (fileArchiveHierarchyType == FileArchiveHierarchyType.Virtual)
        {
          var virtualFolder = new FileSystemEntry(GuidGenerator.Create().ToString());
          virtualFolder.EntryKind = EntryKind.Folder;
          virtualFolder.ArchiveId = entityId;
          virtualFolder.Name = name; // Use archive name as root folder name
          virtualFolder.ParentId = null; // Root folder has no parent

          await virtualFolderRepository.InsertAsync(virtualFolder, true);
          fileArchiveEntity.RootFolderId = virtualFolder.Id;
        }

        var repository = await archiveRepository.InsertAsync(fileArchiveEntity, true);
        result = fileArchiveEntity;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> DeleteFileArchive(Guid id)
    {
      bool result = false;
      try
      {
        var fileArchiveEntity = await archiveRepository.GetAsync(id);
        await archiveRepository.DeleteAsync(id, true);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<FileArchiveEntity> GetFileArchiveById(Guid id)
    {
      FileArchiveEntity result = default;
      try
      {

        if (!await permissionManagement.CheckPermission(id, "", FileManagerPermissionType.Read, FileManagerType.FileArchive))
        {
          throw new AbpAuthorizationException();
        }
        result = await archiveRepository.GetAsync(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<FileArchiveEntity>> GetFileArchivesList()
    {
      List<FileArchiveEntity> result = default;
      try
      {
        result = new List<FileArchiveEntity>();
        var archives = await archiveRepository.GetListAsync(true);

        var providers = await GetStorageProviderListAsync(archives.Select(a => a.StorageProviderId).Distinct().ToList());


        foreach (var archive in archives)
        {
          if (await permissionManagement.CheckPermission(archive.Id, archive.RootFolderId, FileManagerPermissionType.Read, FileManagerType.FileArchive))
          {
            var provider = providers.FirstOrDefault(p => p.Id == archive.StorageProviderId);

            if (provider != null)
            {
              archive.StorageProviderName = provider.Name;
            }

            result.Add(archive);
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<KeyValuePair<long, List<FileArchiveEntity>>> GetFileArchivesListByParams(
       string sorting = null,
       int maxResultCount = int.MaxValue,
       int skipCount = 0,
       string searchQuery = null)
    {
      KeyValuePair<long, List<FileArchiveEntity>> result = new();
      var resultArchives = new List<FileArchiveEntity>();
      var resultCount = 0L;
      try
      {
        var archives = await archiveRepository.GetListAsyncByParams(
                    sorting, maxResultCount, skipCount,
                    searchQuery);
        resultCount = archives.Key;

        if (archives.Value != null && archives.Value.Count > 0)
        {
          var providers = await GetStorageProviderListAsync(archives.Value.Select(a => a.StorageProviderId).Distinct().ToList());
          foreach (var archive in archives.Value)
          {
            if (await permissionManagement.CheckPermission(archive.Id, archive.RootFolderId, FileManagerPermissionType.Read, FileManagerType.FileArchive))
            {
              var provider = providers.FirstOrDefault(p => p.Id == archive.StorageProviderId);
              if (provider != null)
              {
                archive.StorageProviderName = provider.Name;
              }

              resultArchives.Add(archive);
            }
            else
            {
              resultCount--;
            }
          }
        }

        result = new KeyValuePair<long, List<FileArchiveEntity>>(resultCount, resultArchives);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<FileArchiveEntity> UpdateFileArchive(FileArchiveEntity fileArchive)
    {
      FileArchiveEntity result = default;
      try
      {
        var existingEntity = await archiveRepository.GetAsync(fileArchive.Id);

        // Idempotency: check if values are already set (simple check for name and active status)
        var nameChanged = existingEntity.Name != fileArchive.Name;
        var activeChanged = existingEntity.IsActive != fileArchive.IsActive;
        var personalizedChanged = existingEntity.IsPersonalizedArchive != fileArchive.IsPersonalizedArchive;
        var providerChanged = fileArchive.FileArchiveHierarchyType == FileArchiveHierarchyType.Physical &&
          (existingEntity.StorageProviderId != fileArchive.StorageProviderId ||
           existingEntity.PhysicalRootFolderId != fileArchive.PhysicalRootFolderId);

        if (!nameChanged && !activeChanged && !personalizedChanged && !providerChanged)
        {
          logger.Log.LogInformation(
            "File archive {ArchiveId} already has the same values. Treating as idempotent success.",
            fileArchive.Id);
          return existingEntity;
        }

        existingEntity.Name = fileArchive.Name;

        if (fileArchive.FileArchiveHierarchyType == FileArchiveHierarchyType.Physical)
        {
          existingEntity.StorageProviderId = fileArchive.StorageProviderId;
          existingEntity.PhysicalRootFolderId = fileArchive.PhysicalRootFolderId;
        }

        existingEntity.IsActive = fileArchive.IsActive;
        existingEntity.IsPersonalizedArchive = fileArchive.IsPersonalizedArchive;

        try
        {
          result = await archiveRepository.UpdateAsync(existingEntity, true);
        }
        catch (AbpDbConcurrencyException ex)
        {
          logger.Log.LogWarning(
            ex,
            "Concurrency conflict while updating file archive {ArchiveId}. Waiting for desired state...",
            fileArchive.Id);

          var resolved = await ConcurrencyExtensions.WaitForDesiredStateAsync(
            async () =>
            {
              var currentEntity = await archiveRepository.GetAsync(fileArchive.Id);
              var isDesired =
                currentEntity.Name == fileArchive.Name &&
                currentEntity.IsActive == fileArchive.IsActive &&
                currentEntity.IsPersonalizedArchive == fileArchive.IsPersonalizedArchive &&
                (fileArchive.FileArchiveHierarchyType != FileArchiveHierarchyType.Physical ||
                 (currentEntity.StorageProviderId == fileArchive.StorageProviderId &&
                  currentEntity.PhysicalRootFolderId == fileArchive.PhysicalRootFolderId));

              var details =
                $"Name={currentEntity.Name},IsActive={currentEntity.IsActive},IsPersonalized={currentEntity.IsPersonalizedArchive},StorageProviderId={currentEntity.StorageProviderId},PhysicalRootFolderId={currentEntity.PhysicalRootFolderId}";

              return new ConcurrencyExtensions.ConcurrencyWaitResult<FileArchiveEntity>(isDesired, currentEntity, details);
            },
            logger.Log,
            "UpdateFileArchive",
            fileArchive.Id
          );

          result = resolved;
        }
      }
      catch (AbpDbConcurrencyException)
      {
        throw; // Re-throw after handling above
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    private async Task<List<MinimalStorageProviderDto>> GetStorageProviderListAsync(List<Guid> providerIds)
    {
      var providers = await _eventBus.RequestAsync<GetMinimalStorageProvidersListResponseMsg>(new GetMinimalStorageProvidersListMsg() { ProviderIds = providerIds });

      if (!providers.IsSuccess)
      {
        logger.Log.LogError("Failed to get minimal storage providers list.");
        throw new Exception("Failed to get minimal storage providers list.");
      }

      return providers.StorageProviders;
    }
  }
}
