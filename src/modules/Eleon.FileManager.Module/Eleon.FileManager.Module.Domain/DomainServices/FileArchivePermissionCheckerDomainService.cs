using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using Logging.Module;
using MassTransit.Futures.Contracts;
using Microsoft.AspNetCore.Identity;
using Migrations.Module;
using ModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Constants;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.DomainServices
{
  public class FileArchivePermissionCheckerDomainService : DomainService
  {
    private readonly FileArchivePermissionManagementDomainService fileArchivePermissionManagementDomainService;
    private readonly ICurrentUser currentUser;
    private readonly Managers.FileManager fileManager;
    private readonly IVportalLogger<FileArchivePermissionCheckerDomainService> logger;
    private readonly IdentityUserManager identityUserManager;
    private readonly IArchiveRepository archiveRepository;

    public FileArchivePermissionCheckerDomainService(
            FileArchivePermissionManagementDomainService fileArchivePermissionManagementDomainService,
            ICurrentUser currentUser,
            Managers.FileManager fileManager,
            IVportalLogger<FileArchivePermissionCheckerDomainService> logger,
            IdentityUserManager identityUserManager,
            IArchiveRepository archiveRepository
            )
    {
      this.fileArchivePermissionManagementDomainService = fileArchivePermissionManagementDomainService;
      this.currentUser = currentUser;
      this.fileManager = fileManager;
      this.logger = logger;
      this.identityUserManager = identityUserManager;
      this.archiveRepository = archiveRepository;
    }

    public async Task<bool> CheckFilePermission(Guid archiveId, string fileId, FileManagerPermissionType permissionType, FileManagerType type)
    {
      bool result = false;
      try
      {
        if (type == FileManagerType.Provider)
        {
          return true;
        }

        var fileEntity = await fileManager.GetEntryById(fileId, archiveId, type);
        if (fileEntity == null)
        {
          return false;
        }

        string folderId = fileEntity.FolderId; //= x;

        var permissions = await GetAllowedPermissionsForCurrentUser(archiveId, folderId);

        if (permissions != null)
        {
          result = permissions.Contains(permissionType);
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
    public async Task<List<FileSystemEntry>> GetPermited(Guid archiveId, List<FileSystemEntry> entries, FileManagerPermissionType permissionType, FileManagerType type)
    {
      List<FileSystemEntry> result = new List<FileSystemEntry>();
      try
      {
        if (type == FileManagerType.Provider)
        {
          return entries;
        }

        foreach (var entry in entries)
        {
          string folderId = entry.EntryKind == EntryKind.File
              ? (entry.ParentId ?? entry.FolderId) // Use ParentId, fallback to FolderId for legacy
              : entry.ParentId; // For folders, use ParentId

          var permissions = await GetAllowedPermissionsForCurrentUser(archiveId, folderId);

          if (permissions != null && permissions.Contains(permissionType))
          {
            result.Add(entry);
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
    public async Task<bool> CheckPermission(Guid archiveId, string folderId, FileManagerPermissionType permissionType, FileManagerType type)
    {

      bool result = false;
      try
      {
        if (type == FileManagerType.Provider)
        {
          return true; // TO DO check provider level permissions
        }
        var permissions = await GetAllowedPermissionsForCurrentUser(archiveId, folderId);

        if (permissions != null)
        {
          result = permissions.Contains(permissionType);
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
    public Task<IEnumerable<FileManagerPermissionType>> GetAllowedPermissionsForCurrentUser(FileArchivePermissionKeyValueObject key)
        => GetAllowedPermissionsForCurrentUser(key.ArchiveId, key.FolderId);
    public async Task<IEnumerable<FileManagerPermissionType>> GetAllowedPermissions(Guid archiveId, string folderId, Guid userId)
    {

      List<FileManagerPermissionType> result = new List<FileManagerPermissionType>();
      try
      {
        var user = await identityUserManager.GetByIdAsync(userId);

        bool isAdmin = await identityUserManager.IsInRoleAsync(user, MigrationConsts.AdminRoleNameDefaultValue);

        var archive = await archiveRepository.GetAsync(archiveId);

        if (isAdmin || archive.CreatorId == userId)
        {
          return new List<FileManagerPermissionType>() { FileManagerPermissionType.Read, FileManagerPermissionType.Modify, FileManagerPermissionType.Write };
        }

        List<string> parentIds;
        if (string.IsNullOrEmpty(folderId))
        {
          parentIds = new List<string>() { archive.RootFolderId };
        }
        else
        {
          parentIds = await GetParentIds(archiveId, folderId);
        }

        var permissionFilter = parentIds.Concat(new List<string>() { null, });

        var permissions = await fileArchivePermissionManagementDomainService.GetListAsync(archiveId, permissionFilter.ToList());

        List<FileArchivePermissionEntity> selectedPermissionGroup = GetRelevantPermissionGroup(parentIds, permissions);

        if (selectedPermissionGroup != null && selectedPermissionGroup.Count > 0)
        {
          foreach (var permission in selectedPermissionGroup)
          {
            if (Guid.TryParse(permission.ActorId, out Guid actorId))
            {
              if (await CheckActor(permission.ActorType, actorId, user))
              {
                foreach (var allowedPermission in permission.AllowedPermissions)
                {
                  if (!result.Contains(allowedPermission))
                  {
                    result.Add(allowedPermission);
                  }
                }
              }
            }
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

    private List<FileArchivePermissionEntity> GetRelevantPermissionGroup(List<string> parentIds, List<FileArchivePermissionEntity> permissions)
    {

      List<FileArchivePermissionEntity> result = null;
      try
      {
        PriorityQueue<List<FileArchivePermissionEntity>, int> priorityQueue =
                            new PriorityQueue<List<FileArchivePermissionEntity>, int>(permissions.Count, Comparer<int>.Create((x, y) => y.CompareTo(x)));

        var permissionGroups = permissions.GroupBy(
            p => p.FolderId,
            p => p,
            (key, g) => new { FolderId = key, Permissions = g.ToList() });

        foreach (var permissionGroup in permissionGroups)
        {
          int priority = 0;
          if (!string.IsNullOrEmpty(permissionGroup.FolderId))
          {
            bool isDefault = permissionGroup.Permissions.Count() == 0 ||
                (permissionGroup.Permissions.Count == 1 && permissionGroup.Permissions.Any(d => string.IsNullOrEmpty(d.ActorId)));
            int priorityCoefficient = isDefault ? 1 : 2;
            priority += parentIds.IndexOf(permissionGroup.FolderId) * priorityCoefficient;
          }

          priorityQueue.Enqueue(permissionGroup.Permissions, priority);
        }
        if (priorityQueue != null && priorityQueue.Count > 0)
        {
          result = priorityQueue.Dequeue();
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

    private Task<bool> CheckActor(PermissionActorType actorType, Guid actorId, Volo.Abp.Identity.IdentityUser user)
        => Task.FromResult(actorType switch
        {
          PermissionActorType.User => actorId == user.Id,
          PermissionActorType.Role => user.Roles.Any(r => r.RoleId == actorId),
          PermissionActorType.OrganizationUnit => user.OrganizationUnits.Any(r => r.OrganizationUnitId == actorId),
          _ => false
        });

    private async Task<List<string>> GetParentIds(Guid archiveId, string folderId)
    {
      List<string> parentIds = new List<string>();
      if (!string.IsNullOrEmpty(folderId))
      {
        try
        {
          var parents = await fileManager.GetEntryParentsById(folderId, archiveId, FileManagerType.FileArchive);
          if (parents != null && parents.Count > 0)
          {
            parentIds = parents.Select(p => p.Id).ToList();
          }
        }
        catch (Exception e)
        {
          logger.CaptureAndSuppress(e);
        }
      }

      return parentIds;
    }

    public async Task<IEnumerable<FileManagerPermissionType>> GetAllowedPermissionsForCurrentUser(Guid archiveId, string folderId)
        => await GetAllowedPermissions(archiveId, folderId, currentUser.Id.Value);
  }
}
