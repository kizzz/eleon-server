using Volo.Abp.Domain.Repositories;
using VPortal.FileManager.Module.Entities;

namespace VPortal.FileManager.Module.Repositories
{
  public interface IVirtualFolderRepository : IBasicRepository<FileSystemEntry, string>
  {

  }
}
