using Common.Module.Constants;
using Eleon.TenantManagement.Module.Eleon.TenantManagement.Module.Domain.Shared.Consts;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Identity.Module.Entities;
using VPortal.TenantManagement.Module.EntityFrameworkCore;

namespace VPortal.Identity.Module.Repositories
{
  public class CustomCredentialsRepository : EfCoreRepository<TenantManagementDbContext, CustomCredentialsEntity, Guid>, ICustomCredentialsRepository
  {
    private readonly IVportalLogger<CustomCredentialsRepository> logger;

    public CustomCredentialsRepository(
        IDbContextProvider<TenantManagementDbContext> dbContextProvider,
        IVportalLogger<CustomCredentialsRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<CustomCredentialsEntity> GetByValue(CustomCredentialsSet credentialsSet, string value)
    {
      CustomCredentialsEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.CustomCredentials
            .FirstOrDefaultAsync(x => x.CredentialsSet == credentialsSet && x.Value == value);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
