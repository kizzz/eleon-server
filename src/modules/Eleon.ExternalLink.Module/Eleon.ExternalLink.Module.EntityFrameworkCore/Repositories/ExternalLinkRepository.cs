using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.ExternalLink.Module.Entities;
using VPortal.ExternalLink.Module.EntityFrameworkCore;

namespace VPortal.ExternalLink.Module.Repositories
{
  public class ExternalLinkRepository : EfCoreRepository<ExternalLinkDbContext, ExternalLinkEntity, Guid>, IExternalLinkRepository
  {
    private readonly IDbContextProvider<ExternalLinkDbContext> dbContextProvider;
    private readonly IVportalLogger<ExternalLinkRepository> logger;
    public ExternalLinkRepository(IDbContextProvider<ExternalLinkDbContext> dbContextProvider, IVportalLogger<ExternalLinkRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
      this.dbContextProvider = dbContextProvider;
    }

    public override async Task<IQueryable<ExternalLinkEntity>> WithDetailsAsync()
    {
      var result = await base.WithDetailsAsync();

      return result;
    }

    public override Task<ExternalLinkEntity> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
      return base.GetAsync(id, includeDetails, cancellationToken);
    }

    public async Task<ExternalLinkEntity> GetAsync(string linkCode)
    {

      ExternalLinkEntity result = null;
      try
      {
        DbSet<ExternalLinkEntity> dbSet = await GetDbSetAsync();
        result = await dbSet.FirstOrDefaultAsync(t => t.ExternalLinkCode == linkCode);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<List<ExternalLinkEntity>> GetByPrivateParamsAndDocTypeAsync(string privateParams, string documentType)
    {

      List<ExternalLinkEntity> result = null;
      try
      {
        DbSet<ExternalLinkEntity> dbSet = await GetDbSetAsync();
        result = await dbSet.Where(t => t.PrivateParams == privateParams && t.DocumentType == documentType).ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }
  }
}
