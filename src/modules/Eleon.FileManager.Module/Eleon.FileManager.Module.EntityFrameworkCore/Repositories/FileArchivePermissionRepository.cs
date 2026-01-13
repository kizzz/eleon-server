using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.EntityFrameworkCore;
using VPortal.FileManager.Module.Specifications;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Repositories
{
  public class FileArchivePermissionRepository :
      EfCoreRepository<FileManagerDbContext, FileArchivePermissionEntity, Guid>,
      IFileArchivePermissionRepository
  {
    private readonly IVportalLogger<FileArchivePermissionRepository> logger;
    private readonly IDbContextProvider<FileManagerDbContext> dbContextProvider;

    public FileArchivePermissionRepository(
        IVportalLogger<FileArchivePermissionRepository> logger,
        IDbContextProvider<FileManagerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public async Task<List<FileArchivePermissionEntity>> GetListAsync(Guid archiveId, List<string> folderIds)
    {
      List<FileArchivePermissionEntity> result = null;
      try
      {
        var dbSet = await GetDbSetAsync();
        result = dbSet
            .Where(c => c.ArchiveId == archiveId)
            .Where(c => folderIds.Contains(c.FolderId))
            .Include(c => c.PermissionTypes)
            .ToList();
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
        var dbSet = await GetDbSetAsync();
        result = dbSet
            .Include(c => c.PermissionTypes)
            .FirstOrDefault(new FileArchivePermissionKeyMatchSpecification(keyValueObject).ToExpression());
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

    public async Task<FileArchivePermissionEntity> UpdatePermissions(FileArchivePermissionValueObject permissionValueObject)
    {

      FileArchivePermissionEntity result = null;
      try
      {
        result = await GetPermissionWithoutDefault(permissionValueObject);

        if (result == null)
        {
          result = new FileArchivePermissionEntity(permissionValueObject);
          result = await InsertAsync(result, true);
        }

        result.PermissionTypes = permissionValueObject.AllowedPermissions
            .Select(p => new FileArchivePermissionTypeEntity(GuidGenerator.Create(), p))
            .ToList();
        await UpdateAsync(result, true);
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
        var entity = await GetPermissionWithoutDefault(permissionValueObject);

        var dbSet = await GetDbSetAsync();
        var deleted = dbSet.FirstOrDefault(x => x.ActorId == permissionValueObject.ActorId && x.ActorType == permissionValueObject.ActorType &&
            x.FolderId == permissionValueObject.FolderId && x.ArchiveId == permissionValueObject.ArchiveId);
        if (deleted != null)
        {
          await DeleteAsync(deleted, true);
          result = true;
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
  }
}
