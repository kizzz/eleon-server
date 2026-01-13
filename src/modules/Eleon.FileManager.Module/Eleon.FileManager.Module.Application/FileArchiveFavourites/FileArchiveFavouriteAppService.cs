using Logging.Module;
using System;
using System.Threading.Tasks;
using VPortal.FileManager.Module.DomainServices;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.FileArchiveFavourites
{
  public class FileArchiveFavouriteAppService : ModuleAppService, IFileArchiveFavouriteAppService
  {
    private readonly IVportalLogger<FileArchiveFavouriteAppService> logger;
    private readonly FileArchiveFavouriteDomainService domainService;

    public FileArchiveFavouriteAppService(
        IVportalLogger<FileArchiveFavouriteAppService> logger,
        FileArchiveFavouriteDomainService domainService
        )
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<bool> AddToFavourites(FileArchiveFavouriteDto favouriteDto)
    {
      bool result = false;
      try
      {
        var valueObject = ObjectMapper.Map<FileArchiveFavouriteDto, FileArchiveFavouriteValueObject>(favouriteDto);
        result = await domainService.AddToFavourites(valueObject);
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

    public async Task<bool> RemoveFromFavourites(FileArchiveFavouriteDto favouriteDto)
    {
      bool result = false;
      try
      {
        result = await domainService.RemoveFromFavourites(favouriteDto.ArchiveId, favouriteDto.FileId, favouriteDto.FolderId);
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
