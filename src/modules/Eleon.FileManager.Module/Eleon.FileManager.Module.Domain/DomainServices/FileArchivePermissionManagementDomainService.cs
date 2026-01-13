using Logging.Module;
using Microsoft.Extensions.Localization;
using ModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Localization;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.DomainServices
{
  public class FileArchivePermissionManagementDomainService : DomainService
  {
    private readonly IFileArchivePermissionRepository repository;
    private readonly IVportalLogger<FileArchivePermissionManagementDomainService> logger;
    private readonly IdentityUserManager identityUserManager;
    private readonly IdentityRoleManager identityRoleManager;
    private readonly IOrganizationUnitRepository organizationUnitRepository;

    public IStringLocalizer<ModuleResource> L { get; }

    public FileArchivePermissionManagementDomainService(
        IFileArchivePermissionRepository repository,
        IVportalLogger<FileArchivePermissionManagementDomainService> logger,
        IdentityUserManager identityUserManager,
        IdentityRoleManager identityRoleManager,
        IOrganizationUnitRepository organizationUnitRepository,
        IStringLocalizer<ModuleResource> L)
    {
      this.repository = repository;
      this.logger = logger;
      this.identityUserManager = identityUserManager;
      this.identityRoleManager = identityRoleManager;
      this.organizationUnitRepository = organizationUnitRepository;
      this.L = L;
    }

    public async Task<List<FileArchivePermissionEntity>> GetListAsync(Guid archiveId, List<string> folderIds)
    {

      List<FileArchivePermissionEntity> result = null;
      try
      {
        result = await repository.GetListAsync(archiveId, folderIds);

        foreach (var entity in result)
        {
          try
          {
            if (string.IsNullOrEmpty(entity.ActorId))
            {
              entity.ActorDisplayName = L["FailedToGetName"];
            }
            else if (entity.ActorType == PermissionActorType.User)
            {
              entity.ActorDisplayName = (await identityUserManager.FindByIdAsync(entity.ActorId.ToString())).Name;
            }
            else if (entity.ActorType == PermissionActorType.Role)
            {
              entity.ActorDisplayName = (await identityRoleManager.FindByIdAsync(entity.ActorId.ToString())).Name;
            }
            else if (entity.ActorType == PermissionActorType.OrganizationUnit)
            {
              var orgUnit = await organizationUnitRepository.FindAsync(Guid.Parse(entity.ActorId));
              entity.ActorDisplayName = orgUnit?.DisplayName;
            }
          }
          catch (Exception e)
          {
            entity.ActorDisplayName = L["FailedToGetName"];
            logger.CaptureAndSuppress(e);
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
    public async Task<FileArchivePermissionEntity> UpdatePermission(FileArchivePermissionValueObject valueObject)
    {
      FileArchivePermissionEntity result = null;
      try
      {
        result = await repository.UpdatePermissions(valueObject);
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

    public async Task<FileArchivePermissionEntity> GetPermissionWithoutDefault(FileArchivePermissionKeyValueObject keyValueObject)
    {

      FileArchivePermissionEntity result = null;
      try
      {
        result = await repository.GetPermissionWithoutDefault(keyValueObject);
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

    public async Task<bool> DeletePermissions(FileArchivePermissionValueObject permissionValueObject)
    {
      bool result = false;
      try
      {
        result = await repository.DeletePermissions(permissionValueObject);
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
  }
}
