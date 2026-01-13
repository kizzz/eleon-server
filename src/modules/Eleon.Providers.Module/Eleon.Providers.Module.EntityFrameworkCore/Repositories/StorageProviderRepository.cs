using Logging.Module;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Storage.Module.Entities;
using VPortal.Storage.Module.EntityFrameworkCore;
using VPortal.Storage.Module.Extensions;

namespace VPortal.Storage.Module.Repositories
{
  public class StorageProviderRepository :
      EfCoreRepository<StorageDbContext, StorageProviderEntity, Guid>,
      IStorageProviderRepository
  {
    private readonly IVportalLogger<StorageProviderRepository> logger;

    public StorageProviderRepository(
        IDbContextProvider<StorageDbContext> dbContextProvider,
        IVportalLogger<StorageProviderRepository> logger) : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public override async Task<IQueryable<StorageProviderEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync()).IncludeDetails();
    }


    public Task<StorageProviderEntity> GetByIdWithCache(string id)
    {
      throw new NotImplementedException();
    }

    public Task<bool> RemoveFromCache(string id)
    {
      throw new NotImplementedException();
    }

    public Task<bool> AddToCache(string id, StorageProviderEntity entity)
    {
      throw new NotImplementedException();
    }

    public async Task<List<StorageProviderEntity>> GetStorageProvidersList(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null)
    {
      //KeyValuePair<int, List<StorageProviderEntity>> result = default;
      List<StorageProviderEntity> result = default;
      try
      {
        var queryable = await GetQueryableAsync();
        string pattern = searchQuery == null ? null : $"%{searchQuery}%";

        var filtered = queryable
            .WhereIf(searchQuery != null, x => EF.Functions.Like(x.Name, pattern))
            //.WhereIf(searchQuery != null,
            //    x => EF.Functions.Like(x.ExpenseTypeName, pattern)
            //        || EF.Functions.Like(x.ExpenseTypeExternalCode, pattern))
            .OrderByIf<StorageProviderEntity, IQueryable<StorageProviderEntity>>(sorting != null, sorting);

        var paged = filtered
            .Skip(skipCount)
            .Take(maxResultCount);

        var entities = await paged.ToListAsync();
        //var count = await filtered.CountAsync();

        //result = new(count, entities);
        result = entities;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<StorageProviderEntity>> GetStorageProvidersListByIds(List<Guid> ids)
    {
      //KeyValuePair<int, List<StorageProviderEntity>> result = default;
      List<StorageProviderEntity> result = default;
      try
      {
        var queryable = await GetQueryableAsync();

        var filtered = queryable
            .Where(x => ids.Contains(x.Id));

        var entities = await filtered.ToListAsync();
        result = entities;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }
  }
}
