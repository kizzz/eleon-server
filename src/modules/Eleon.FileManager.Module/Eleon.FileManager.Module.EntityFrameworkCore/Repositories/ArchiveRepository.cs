using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.FileManager.Module.Entities;
using VPortal.FileManager.Module.EntityFrameworkCore;

namespace VPortal.FileManager.Module.Repositories
{
  public class ArchiveRepository :
      EfCoreRepository<FileManagerDbContext, FileArchiveEntity, Guid>,
      IArchiveRepository
  {
    private readonly IVportalLogger<ArchiveRepository> logger;
    private readonly IDbContextProvider<FileManagerDbContext> dbContextProvider;

    public ArchiveRepository(
        IVportalLogger<ArchiveRepository> logger,
        IDbContextProvider<FileManagerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public async Task<KeyValuePair<long, List<FileArchiveEntity>>> GetListAsyncByParams(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null)
    {
      KeyValuePair<long, List<FileArchiveEntity>> result = new();
      try
      {
        var dbContext = await GetDbContextAsync();
        string pattern = searchQuery == null ? null : $"%{searchQuery}%";
        var filtered = dbContext.FileArchives
            .WhereIf(searchQuery != null,
                x => EF.Functions.Like(x.Name, pattern));

        var paginated = filtered
           .OrderBy(sorting)
           .ThenByDescending(x => x.CreationTime)
           .Skip(skipCount)
           .Take(maxResultCount);
        result = new(await filtered.CountAsync(), await paginated.ToListAsync());
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
