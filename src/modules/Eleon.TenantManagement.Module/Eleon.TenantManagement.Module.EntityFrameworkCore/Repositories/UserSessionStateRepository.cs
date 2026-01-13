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
  public class UserSessionStateRepository : EfCoreRepository<TenantManagementDbContext, UserSessionStateEntity, Guid>, IUserSessionStateRepository
  {
    private readonly IVportalLogger<UserSessionStateRepository> logger;

    public UserSessionStateRepository(
        IDbContextProvider<TenantManagementDbContext> dbContextProvider,
        IVportalLogger<UserSessionStateRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<UserSessionStateEntity> GetByUser(Guid userId)
    {
      UserSessionStateEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        result = await dbContext.UserSessionStates
            .FirstOrDefaultAsync(x => x.UserId == userId);
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
