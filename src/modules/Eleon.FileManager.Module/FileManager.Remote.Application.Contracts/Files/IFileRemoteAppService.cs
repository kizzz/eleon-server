using Volo.Abp.Application.Services;
using VPortal.FileManager.Module.Repositories;

namespace FileManager.Remote.Application.Contracts.Files
{
  public interface IFileRemoteAppService : IFileRepository, IApplicationService
  {
  }
}
