using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.FileManager.Module.FileArchiveFavourites
{
  public interface IFileArchiveFavouriteAppService : IApplicationService
  {
    Task<bool> AddToFavourites(FileArchiveFavouriteDto favouriteDto);
    Task<bool> RemoveFromFavourites(FileArchiveFavouriteDto favouriteDto);
  }
}
