using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.EntityFrameworkCore;

namespace VPortal.FileManager.Module.Repositories
{
  public class FileExternalLinkRepository : EfCoreRepository<FileManagerDbContext, FileExternalLinkEntity, Guid>, IFileExternalLinkRepository
  {
    private readonly IDbContextProvider<FileManagerDbContext> dbContextProvider;
    private readonly IVportalLogger<FileExternalLinkRepository> logger;
    public FileExternalLinkRepository(IDbContextProvider<FileManagerDbContext> dbContextProvider, IVportalLogger<FileExternalLinkRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public override async Task<IQueryable<FileExternalLinkEntity>> WithDetailsAsync()
    {
      var result = await base.WithDetailsAsync();

      result.Include(r => r.Reviewers);

      return result;
    }

    public override Task<FileExternalLinkEntity> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
      return base.GetAsync(id, includeDetails, cancellationToken);
    }

  }
}
