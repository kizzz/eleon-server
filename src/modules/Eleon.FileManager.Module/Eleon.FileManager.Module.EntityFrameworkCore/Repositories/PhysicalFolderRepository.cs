using Logging.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.EntityFrameworkCore;

namespace VPortal.FileManager.Module.Repositories
{
  public class PhysicalFolderRepository : EfCoreRepository<FileManagerDbContext, PhysicalFolderEntity, string>, IPhysicalFolderRepository
  {
    private readonly IDbContextProvider<FileManagerDbContext> dbContextProvider;
    private readonly IVportalLogger<PhysicalFolderRepository> logger;
    public PhysicalFolderRepository(IDbContextProvider<FileManagerDbContext> dbContextProvider, IVportalLogger<PhysicalFolderRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public Task GetParentFolderPath(string id)
    {
      throw new NotImplementedException();
    }
  }
}
