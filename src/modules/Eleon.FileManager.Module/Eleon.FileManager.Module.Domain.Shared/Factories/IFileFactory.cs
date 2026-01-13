using Common.Module.Constants;
using EleonsoftModuleCollector.FileManager.Module.FileManager.Module.Domain.Shared.Constants;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.Repositories;

namespace VPortal.FileManager.Module.Factories
{
  public interface IFileFactory : ITransientDependency
  {
    public Task<IFileSystemEntryRepository> Get(FileArchiveEntity fileArchiveEntity, FileManagerType type);
    public IFileSystemEntryRepository Get(FileArchiveHierarchyType fileArchiveHierarchyType);
  }
}
