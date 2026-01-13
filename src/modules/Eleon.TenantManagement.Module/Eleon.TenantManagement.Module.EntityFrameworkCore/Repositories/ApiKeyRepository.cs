using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Identity.Module.Entities;
using VPortal.TenantManagement.Module.EntityFrameworkCore;

namespace VPortal.Identity.Module.Repositories
{
  public class ApiKeyRepository : EfCoreRepository<TenantManagementDbContext, ApiKeyEntity, Guid>, IApiKeyRepository
  {
    private readonly IVportalLogger<ApiKeyRepository> logger;

    public ApiKeyRepository(
        IDbContextProvider<TenantManagementDbContext> dbContextProvider,
        IVportalLogger<ApiKeyRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<ApiKeyEntity> GetByKey(string key)
    {
      ApiKeyEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.ApiKeys
            .FirstOrDefaultAsync(x => x.Key == key);
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

    public async Task<ApiKeyEntity> GetBySubject(ApiKeyType keyType, string subject)
    {
      ApiKeyEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.ApiKeys
            .FirstOrDefaultAsync(x => x.Type == keyType && x.RefId == subject);
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
