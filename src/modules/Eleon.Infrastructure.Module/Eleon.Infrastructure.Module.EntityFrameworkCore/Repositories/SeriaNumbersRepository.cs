using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Infrastructure.Module.Entities;
using VPortal.Infrastructure.Module.EntityFrameworkCore;
using VPortal.Shared.Modules.Core.Module.Repositories;

namespace VPortal.Infrastructure.Module.Repositories
{
  public class SeriaNumbersRepository : EfCoreRepository<InfrastructureDbContext, SeriaNumberEntity, Guid>, ISeriaNumbersRepository
  {
    private readonly IVportalLogger<SeriaNumbersRepository> _logger;

    public SeriaNumbersRepository(
        IDbContextProvider<InfrastructureDbContext> dbContextProvider,
        IVportalLogger<SeriaNumbersRepository> logger)
        : base(dbContextProvider)
    {
      _logger = logger;
    }

    public async Task<SeriaNumberEntity> GetSeriaNumberAsync(string objectType, string refId, string prefix)
    {

      SeriaNumberEntity result = null;
      try
      {
        DbSet<SeriaNumberEntity> dbSet = await GetDbSetAsync();

        result = await dbSet.FirstOrDefaultAsync(
            x => x.Prefix == prefix && x.ObjectType == objectType && x.RefId == refId);
      }
      catch (Exception e)
      {
        _logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }
  }
}
