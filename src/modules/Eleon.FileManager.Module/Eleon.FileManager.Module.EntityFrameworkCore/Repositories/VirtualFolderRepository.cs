using Logging.Module;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.EntityFrameworkCore;

namespace VPortal.FileManager.Module.Repositories
{
  public class VirtualFolderRepository : EfCoreRepository<FileManagerDbContext, FileSystemEntry, string>, IVirtualFolderRepository
  {
    private readonly IDbContextProvider<FileManagerDbContext> dbContextProvider;
    private readonly IVportalLogger<VirtualFolderRepository> logger;
    public VirtualFolderRepository(IDbContextProvider<FileManagerDbContext> dbContextProvider, IVportalLogger<VirtualFolderRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }
  }
}
