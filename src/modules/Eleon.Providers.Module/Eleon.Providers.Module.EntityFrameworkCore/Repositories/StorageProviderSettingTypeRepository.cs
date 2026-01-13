using Logging.Module;
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
  public class StorageProviderSettingTypeRepository :
      EfCoreRepository<StorageDbContext, StorageProviderSettingTypeEntity, Guid>, IStorageProviderSettingTypeRepository
  {
    private readonly IVportalLogger<StorageProviderSettingTypeRepository> logger;

    public StorageProviderSettingTypeRepository(
        IDbContextProvider<StorageDbContext> dbContextProvider,
        IVportalLogger<StorageProviderSettingTypeRepository> logger) : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<List<StorageProviderSettingTypeEntity>> GetListByStorageProviderType(string storageProviderType)
    {
      List<StorageProviderSettingTypeEntity> result = null;
      try
      {
        var queryable = await GetQueryableAsync();
        result = await queryable
            .Where(x => x.StorageProviderTypeName == storageProviderType)
            .ToListAsync();
      }
      catch (Exception e)
      {
        logger.Capture(e);
        return new List<StorageProviderSettingTypeEntity>();
      }
      finally
      {
      }

      return result;
    }
  }
}
