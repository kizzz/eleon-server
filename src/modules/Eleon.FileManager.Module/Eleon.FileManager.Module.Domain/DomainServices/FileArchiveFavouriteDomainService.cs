using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.DomainServices
{
  public class FileArchiveFavouriteDomainService : DomainService
  {
    private readonly IVportalLogger<FileArchiveFavouriteDomainService> logger;
    private readonly IFileArchiveFavouriteRepository repository;
    private readonly ICurrentUser currentUser;

    public FileArchiveFavouriteDomainService(
        IVportalLogger<FileArchiveFavouriteDomainService> logger,
        IFileArchiveFavouriteRepository repository,
        ICurrentUser currentUser
        )
    {
      this.logger = logger;
      this.repository = repository;
      this.currentUser = currentUser;
    }

    public async Task<bool> RemoveFromFavourites(Guid archiveId, string fileId, string folderId)
    {
      bool result = false;
      try
      {
        var favouriteEntity = await repository.GetAsync(archiveId, fileId, folderId, currentUser.Id.ToString());

        if (favouriteEntity == null)
        {
          return true;
        }

        await repository.DeleteAsync(favouriteEntity);

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

    public async Task<bool> AddToFavourites(FileArchiveFavouriteValueObject fileArchiveFavouriteValueObject)
    {

      bool result = false;
      try
      {
        fileArchiveFavouriteValueObject.UserId = currentUser.Id.ToString();

        var favouriteEntity = await repository.GetAsync(fileArchiveFavouriteValueObject.ArchiveId, fileArchiveFavouriteValueObject.FileId, fileArchiveFavouriteValueObject.FolderId, fileArchiveFavouriteValueObject.UserId);

        favouriteEntity = new FileArchiveFavouriteEntity(GuidGenerator.Create(), fileArchiveFavouriteValueObject.ArchiveId, fileArchiveFavouriteValueObject.FileId, fileArchiveFavouriteValueObject.FolderId, fileArchiveFavouriteValueObject.ParentId, fileArchiveFavouriteValueObject.UserId);
        favouriteEntity = await repository.InsertAsync(favouriteEntity, true);

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

    public async Task<List<FileArchiveFavouriteEntity>> GetListAsync(Guid archiveId, string parentId)
    {

      List<FileArchiveFavouriteEntity> result = null;
      try
      {
        result = await repository.GetListAsync(archiveId, parentId, currentUser.Id.ToString());
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
