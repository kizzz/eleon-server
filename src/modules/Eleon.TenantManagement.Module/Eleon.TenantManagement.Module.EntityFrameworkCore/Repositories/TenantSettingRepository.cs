using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.EntityFrameworkCore;

namespace VPortal.TenantManagement.Module.Repositories
{
  public class TenantSettingRepository : EfCoreRepository<TenantManagementDbContext, TenantSettingEntity, Guid>, ITenantSettingRepository
  {
    private readonly IVportalLogger<TenantSettingRepository> logger;

    public TenantSettingRepository(
        IDbContextProvider<TenantManagementDbContext> dbContextProvider,
        IVportalLogger<TenantSettingRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<TenantSettingEntity> GetByTenantId(Guid? tenantId)
    {
      TenantSettingEntity result = null;
      try
      {
        var settings = await WithDetailsAsync();
        result = await settings.FirstOrDefaultAsync(x => x.TenantId == tenantId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public override async Task<IQueryable<TenantSettingEntity>> WithDetailsAsync()
        => (await GetDbContextAsync())
        .TenantSettings
        .Include(x => x.Hostnames)
        .Include(x => x.ExternalProviders)
        .Include(x => x.WhitelistedIps)
        .Include(x => x.ContentSecurityHosts)
        .Include(x => x.AppearanceSettings);
  }
}
