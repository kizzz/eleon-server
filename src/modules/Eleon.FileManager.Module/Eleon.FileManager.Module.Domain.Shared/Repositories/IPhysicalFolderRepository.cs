using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.FileManager.Module.Entities;

namespace VPortal.FileManager.Module.Repositories
{
  public interface IPhysicalFolderRepository : IBasicRepository<PhysicalFolderEntity, string>
  {
    Task GetParentFolderPath(string id);
  }
}
