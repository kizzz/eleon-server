using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.FileManager.Module.FileArchiveFavourites;

namespace VPortal.FileManager.Module.Controllers
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/file-manager/file-archives-favourites")]
  public class FileArchiveFavouriteController : ModuleController, IFileArchiveFavouriteAppService
  {
    private readonly IVportalLogger<FileArchiveFavouriteController> _logger;
    private readonly IFileArchiveFavouriteAppService _appService;

    public FileArchiveFavouriteController(
        IVportalLogger<FileArchiveFavouriteController> logger,
        IFileArchiveFavouriteAppService appService)
    {
      _logger = logger;
      _appService = appService;
    }

    [HttpPost("AddToFavourites")]
    public async Task<bool> AddToFavourites(FileArchiveFavouriteDto favouriteDto)
    {

      var response = await _appService.AddToFavourites(favouriteDto);


      return response;
    }

    [HttpDelete("RemoveFromFavourites")]
    public async Task<bool> RemoveFromFavourites(FileArchiveFavouriteDto favouriteDto)
    {

      var response = await _appService.RemoveFromFavourites(favouriteDto);


      return response;
    }
  }
}
