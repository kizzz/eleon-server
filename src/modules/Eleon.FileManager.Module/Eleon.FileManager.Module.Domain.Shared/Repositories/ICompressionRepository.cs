using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.FileManager.Module.ValueObjects;

namespace VPortal.FileManager.Module.Repositories
{
  public interface ICompressionRepository : ITransientDependency
  {
    public Task<byte[]> Compress(List<HierarchyFolderValueObject> virtualFolders, List<FileValueObject> files);
  }
}
