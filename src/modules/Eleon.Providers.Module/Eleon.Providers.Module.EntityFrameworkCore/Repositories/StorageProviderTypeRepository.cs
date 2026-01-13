using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Entities;
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
  public class StorageProviderTypeRepository :
      EfCoreRepository<StorageDbContext, StorageProviderTypeEntity, Guid>, IStorageProviderTypeRepository
  {
    private readonly IVportalLogger<StorageProviderTypeRepository> logger;

    public StorageProviderTypeRepository(
        IDbContextProvider<StorageDbContext> dbContextProvider,
        IVportalLogger<StorageProviderTypeRepository> logger) : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<StorageProviderTypeEntity> GetByName(string name)
    {
      StorageProviderTypeEntity result = default;
      try
      {
        var dbContext = await GetDbContextAsync();

        result = await dbContext.StorageProviderTypes
            .FirstAsync(x => x.Name == name);

        if (result == null)
        {
          throw new Exception($"Storage provider type '{name}' not found.");
        }

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
